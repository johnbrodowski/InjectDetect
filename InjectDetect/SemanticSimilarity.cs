using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class SemanticSimilarity
    {
        private static string[] Tokenize(string text)
        {
            return Regex.Matches(text.ToLowerInvariant(), @"[a-z]+")
                        .Select(m => m.Value)
                        .ToArray();
        }

        // Raw term frequency vector (no IDF — anchors all comparisons to same vocab space)
        private static Dictionary<string, double> TermFrequency(string[] tokens)
        {
            var tf = new Dictionary<string, double>();
            foreach (string t in tokens)
            {
                tf.TryGetValue(t, out double count);
                tf[t] = count + 1;
            }
            int total = tokens.Length;
            if (total > 0)
                foreach (string k in tf.Keys.ToList())
                    tf[k] /= total;
            return tf;
        }

        private static double CosineSimilarity(Dictionary<string, double> a, Dictionary<string, double> b)
        {
            double dot = 0, magA = 0, magB = 0;
            foreach (var kv in a)
            {
                magA += kv.Value * kv.Value;
                if (b.TryGetValue(kv.Key, out double bVal))
                    dot += kv.Value * bVal;
            }
            foreach (var kv in b)
                magB += kv.Value * kv.Value;

            double denom = Math.Sqrt(magA) * Math.Sqrt(magB);
            return denom < 1e-10 ? 0.0 : dot / denom;
        }

        public record VariantScore(string Label, double SimilarityToOriginal, double DriftFromOriginal);

        public record SimilarityReport(
            double SpreadScore,          // max drift from original across all variants
            double AvgDrift,             // average drift from original
            double StdDev,               // stddev of drift scores — high = inconsistent normalization
            List<VariantScore> Scores,   // per-variant breakdown
            string MostDivergentLabel    // which variant drifted most
        );

        // All comparisons are original vs each variant — not pairwise between variants
        public static SimilarityReport Analyze(IList<(string Label, string Text)> variants)
        {
            if (variants.Count < 2)
                return new SimilarityReport(0, 0, 0, new List<VariantScore>(), "");

            // First entry must be the original
            var originalVec = TermFrequency(Tokenize(variants[0].Text));

            var scores = new List<VariantScore>();
            foreach (var (label, text) in variants.Skip(1))
            {
                var vec = TermFrequency(Tokenize(text));
                double sim = CosineSimilarity(originalVec, vec);
                double drift = 1.0 - sim;
                scores.Add(new VariantScore(label, sim, drift));
            }

            double maxDrift = scores.Max(s => s.DriftFromOriginal);
            double avgDrift = scores.Average(s => s.DriftFromOriginal);

            // StdDev of drift — high stddev means some transforms changed meaning a lot, others didn't
            double variance = scores.Average(s => Math.Pow(s.DriftFromOriginal - avgDrift, 2));
            double stdDev = Math.Sqrt(variance);

            string mostDivergent = scores.OrderByDescending(s => s.DriftFromOriginal).First().Label;

            return new SimilarityReport(maxDrift, avgDrift, stdDev, scores, mostDivergent);
        }
    }
}