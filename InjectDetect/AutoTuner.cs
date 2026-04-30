using System;
using System.Collections.Generic;
using System.Linq;

namespace InjectDetect
{
    public static class AutoTuner
    {
        public record SettingsCombo(
            bool RemoveStopWords,
            bool NormalizeSynonyms,
            bool ExpandContractions,
            bool ContractExpanded,
            bool NormalizeWhitespace,
            bool LowercaseVariant,
            bool StripPunctuation,
            bool NormalizeLeetspeak,
            bool RunCombinedVariant,
            bool ExtractQuotedContent,
            bool DecodeBase64,
            bool NumbersToWords,
            bool FilterInvisibleUnicode
        );

        public record TuningResult(
            SettingsCombo Best,
            double Margin,
            double Threshold,
            double Accuracy,
            double TruePositiveRate,
            double FalsePositiveRate,
            int TotalCombinations,
            List<PromptResult> PromptResults
        );

        public record PromptResult(
            string Text,
            PromptClass Class,
            PromptFamily Family,
            int Difficulty,
            ExpectedOutcome Expected,
            double Score,
            DetectionResult Detection,
            bool Correct
        );

        // Per-sentence combo log entry
        public record ComboLogEntry(
            SettingsCombo Combo,
            string ComboLabel,
            double DriftScore,
            double KeywordScore,
            double CompositeScore,
            bool AnyDrift,
            bool Flagged
        );

        public record SentenceLog(
            string Text,
            PromptClass Class,
            int Difficulty,
            int TotalCombos,
            int FlaggedCount,
            int DriftCount,
            double BestScore,
            string BestComboLabel,
            double WorstScore,
            string WorstComboLabel,
            List<ComboLogEntry> Entries
        );

        private record PromptCache(
            string Original,
            string Lowercase,
            string PunctuationStripped,
            string WhitespaceNormalized,
            string ContractionsExpanded,
            string ContractionsContracted,
            string StopWordsRemoved,
            string SynonymsNormalized,
            string StopsAndSynonyms,
            string ExpandedAndSynonyms,
            string ExpandedStopsSynonyms,
            string? QuotedContent,
            string? Base64DecodedAlone,
            string? Base64Substituted,
            string? Base64SubSynonyms,
            string? Base64DecSynonyms,
            string? NumbersToWords,
            string? Leetspeak,
            string? LeetspeakSynonyms,
            string? InvisUnicode,
            string? InvisUnicodeSynonyms,
            Base64Detector.Base64Result? B64Result,
            double IntentScore
        );

        private static PromptCache BuildCache(string text)
        {
            string lower = text.ToLowerInvariant();
            string nopunct = System.Text.RegularExpressions.Regex.Replace(text, @"[^\w\s]", " ");
            string nowhite = System.Text.RegularExpressions.Regex.Replace(
                                  System.Text.RegularExpressions.Regex.Replace(
                                    text, @"[\u200B\u200C\u200D\u00AD\uFEFF\u00A0]", " "),
                                  @" {2,}", " ").Trim();
            string expanded = ContractionNormalizer.Expand(text);
            string contracted = ContractionNormalizer.Contract(text);
            string nostops = StopWordFilter.Filter(text);
            string synonyms = SynonymNormalizer.Normalize(text);
            string stopsyn = SynonymNormalizer.Normalize(nostops);
            string expsyn = SynonymNormalizer.Normalize(expanded);
            string expstopsyn = SynonymNormalizer.Normalize(StopWordFilter.Filter(expanded));
            string? quoted = VariantPipeline.ExtractQuotedContent(text);

            var b64 = Base64Detector.Detect(text);
            string? b64Alone = b64?.DecodedAlone;
            string? b64Sub = b64?.Substituted;
            string? b64SubSyn = b64Sub != null ? SynonymNormalizer.Normalize(b64Sub) : null;
            string? b64DecSyn = b64Alone != null ? SynonymNormalizer.Normalize(b64Alone) : null;
            string? numsToWords = NumberNormalizer.Normalize(text) is var ntw && ntw != text ? ntw : null;
            string leetNorm = LeetSpeakNormalizer.Normalize(text);
            string? leet = leetNorm != text ? leetNorm : null;
            string? leetSyn = leet != null ? SynonymNormalizer.Normalize(leetNorm) : null;
            if (leetSyn == leetNorm || leetSyn == text) leetSyn = null;

            string invisStripped = InvisibleUnicodeFilter.Strip(text);
            string? invisUnicode = invisStripped != text ? invisStripped : null;
            string? invisUnicodeSyn = invisUnicode != null ? SynonymNormalizer.Normalize(invisStripped) : null;
            if (invisUnicodeSyn == invisStripped || invisUnicodeSyn == text) invisUnicodeSyn = null;

            var (intentScore, _) = IntentPatternScorer.Score(text);

            return new PromptCache(text, lower, nopunct, nowhite, expanded, contracted,
                                   nostops, synonyms, stopsyn, expsyn, expstopsyn,
                                   quoted, b64Alone, b64Sub, b64SubSyn, b64DecSyn,
                                   numsToWords, leet, leetSyn, invisUnicode, invisUnicodeSyn,
                                   b64, intentScore);
        }

        private static List<(string Label, string Text)> VariantsFromCache(
            PromptCache c, SettingsCombo s)
        {
            var v = new List<(string, string)> { ("Original", c.Original) };

            if (s.NormalizeWhitespace && c.WhitespaceNormalized != c.Original) v.Add(("Whitespace", c.WhitespaceNormalized));
            if (s.LowercaseVariant && c.Lowercase != c.Original) v.Add(("Lowercase", c.Lowercase));
            if (s.StripPunctuation && c.PunctuationStripped != c.Original) v.Add(("NoPunct", c.PunctuationStripped));
            if (s.ExpandContractions && c.ContractionsExpanded != c.Original) v.Add(("Expanded", c.ContractionsExpanded));
            if (s.ContractExpanded && c.ContractionsContracted != c.Original) v.Add(("Contracted", c.ContractionsContracted));
            if (s.RemoveStopWords && c.StopWordsRemoved != c.Original) v.Add(("NoStops", c.StopWordsRemoved));
            if (s.NormalizeSynonyms && c.SynonymsNormalized != c.Original) v.Add(("Synonyms", c.SynonymsNormalized));

            if (s.RunCombinedVariant)
            {
                if (s.RemoveStopWords && s.NormalizeSynonyms && c.StopsAndSynonyms != c.Original) v.Add(("NoStops+Syn", c.StopsAndSynonyms));
                if (s.ExpandContractions && s.NormalizeSynonyms && c.ExpandedAndSynonyms != c.Original) v.Add(("Exp+Syn", c.ExpandedAndSynonyms));
                if (s.ExpandContractions && s.RemoveStopWords && s.NormalizeSynonyms && c.ExpandedStopsSynonyms != c.Original) v.Add(("Exp+NoStops+Syn", c.ExpandedStopsSynonyms));
            }

            if (s.ExtractQuotedContent && c.QuotedContent != null && c.QuotedContent != c.Original)
                v.Add(("Quoted", c.QuotedContent));

            if (s.NumbersToWords && c.NumbersToWords != null && c.NumbersToWords != c.Original)
                v.Add(("NumWords", c.NumbersToWords));

            if (s.NormalizeLeetspeak && c.Leetspeak != null && c.Leetspeak != c.Original)
                v.Add(("Leet", c.Leetspeak));

            if (s.RunCombinedVariant && s.NormalizeLeetspeak && s.NormalizeSynonyms && c.LeetspeakSynonyms != null && c.LeetspeakSynonyms != c.Original)
                v.Add(("Leet+Syn", c.LeetspeakSynonyms));

            if (s.FilterInvisibleUnicode && c.InvisUnicode != null && c.InvisUnicode != c.Original)
                v.Add(("InvisUnicode", c.InvisUnicode));

            if (s.RunCombinedVariant && s.FilterInvisibleUnicode && s.NormalizeSynonyms && c.InvisUnicodeSynonyms != null && c.InvisUnicodeSynonyms != c.Original)
                v.Add(("InvisUnicode+Syn", c.InvisUnicodeSynonyms));

            if (s.DecodeBase64)
            {
                if (c.Base64DecodedAlone != null && c.Base64DecodedAlone != c.Original)
                    v.Add(("B64Dec", c.Base64DecodedAlone));
                if (c.Base64Substituted != null && c.Base64Substituted != c.Original
                    && c.Base64Substituted != c.Base64DecodedAlone)
                    v.Add(("B64Sub", c.Base64Substituted));
                if (c.Base64SubSynonyms != null && c.Base64SubSynonyms != c.Base64Substituted
                    && c.Base64SubSynonyms != c.Original)
                    v.Add(("B64Sub+Syn", c.Base64SubSynonyms));
                if (c.Base64DecSynonyms != null && c.Base64DecSynonyms != c.Base64DecodedAlone
                    && c.Base64DecSynonyms != c.Original)
                    v.Add(("B64Dec+Syn", c.Base64DecSynonyms));
            }

            return v;
        }

        // Get the most-normalized text from a variant list (last combined pass, or last entry)
        private static string MostNormalized(List<(string Label, string Text)> variants)
        {
            // Prefer combined passes, otherwise last non-original variant
            var combined = variants.Where(v => v.Label.Contains('+') || v.Label.Contains("Syn")).ToList();
            if (combined.Any()) return combined.Last().Text;
            return variants.Count > 1 ? variants.Last().Text : variants[0].Text;
        }

        private static double CompositeScore(
            List<(string Label, string Text)> variants,
            TuningWeights? w = null,
            Base64Detector.Base64Result? b64 = null,
            double intentScore = 0)
        {
            if (variants.Count < 2) return intentScore * (w?.IntentWeight ?? 0);
            w ??= new TuningWeights();

            var report = SemanticSimilarity.Analyze(variants);
            double drift = (report.SpreadScore * w.MaxDriftWeight)
                            + (report.AvgDrift * w.AvgDriftWeight)
                            + (report.StdDev * w.StdDevWeight);
            double keyword = KeywordScorer.Score(MostNormalized(variants), b64);

            return (drift * w.DriftWeight) + (keyword * w.KeywordWeight) + (intentScore * w.IntentWeight);
        }

        public static string ComboLabel(SettingsCombo c)
        {
            var parts = new List<string>();
            if (c.RemoveStopWords) parts.Add("SW");
            if (c.NormalizeSynonyms) parts.Add("SY");
            if (c.ExpandContractions) parts.Add("EX");
            if (c.ContractExpanded) parts.Add("CT");
            if (c.NormalizeWhitespace) parts.Add("WS");
            if (c.LowercaseVariant) parts.Add("LC");
            if (c.StripPunctuation) parts.Add("PU");
            if (c.NormalizeLeetspeak) parts.Add("LT");
            if (c.RunCombinedVariant) parts.Add("CM");
            if (c.ExtractQuotedContent) parts.Add("QC");
            if (c.DecodeBase64) parts.Add("B64");
            if (c.NumbersToWords) parts.Add("N2W");
            if (c.FilterInvisibleUnicode) parts.Add("IU");
            return string.Join("+", parts);
        }

        private static double FindBestThreshold(List<(PromptClass Class, double Score)> scores)
        {
            var candidates = scores.Select(s => s.Score).Distinct().OrderBy(x => x).ToList();
            candidates.Add(0); candidates.Add(1);
            double bestF = -1, bestT = 0.3;
            foreach (double t in candidates)
            {
                var (_, tpr, fpr) = EvaluateAtThreshold(scores, t);
                double beta = 2.0;
                double f = (1 + beta * beta) * tpr * (1 - fpr) /
                           ((beta * beta) * (1 - fpr) + tpr + 1e-10);
                if (f > bestF) { bestF = f; bestT = t; }
            }
            return bestT;
        }

        private static (double Accuracy, double TPR, double FPR) EvaluateAtThreshold(
            List<(PromptClass Class, double Score)> scores, double threshold)
        {
            int tp = 0, fp = 0, tn = 0, fn = 0;
            foreach (var (cls, score) in scores)
            {
                bool predicted = score >= threshold;
                bool actual = cls == PromptClass.Injection;
                if (actual && predicted) tp++;
                else if (!actual && predicted) fp++;
                else if (actual && !predicted) fn++;
                else tn++;
            }
            int total = scores.Count;
            double acc = (double)(tp + tn) / total;
            double tpr = (tp + fn) > 0 ? (double)tp / (tp + fn) : 0;
            double fpr = (fp + tn) > 0 ? (double)fp / (fp + tn) : 0;
            return (acc, tpr, fpr);
        }

        private static bool MeetsExpectedOutcome(DetectionResult detection, ExpectedOutcome expected) =>
            expected switch
            {
                ExpectedOutcome.MustStayClean => detection == DetectionResult.Clean,
                ExpectedOutcome.ShouldBeUncertainOrHigher => detection != DetectionResult.Clean,
                ExpectedOutcome.ShouldBeSuspicious => detection == DetectionResult.Suspicious,
                _ => false,
            };

        private static double PromptWeight(TestPrompt prompt)
        {
            double weight = 1.0 + Math.Max(0, prompt.Difficulty - 2) * 0.2;
            if (prompt.Family == PromptFamily.AdversarialClean) weight += 0.4;
            if (prompt.Family is PromptFamily.SocialEngineering or PromptFamily.SelfReferentialExtraction
                or PromptFamily.AuthorityOrAudit or PromptFamily.MetaAttack)
                weight += 0.3;
            if (prompt.Expected == ExpectedOutcome.ShouldBeSuspicious) weight += 0.2;
            return weight;
        }

        // ── Main tuning run ───────────────────────────────────────────────────────

        public static TuningResult Run(TestPrompt[] corpus, Action<int, int>? progress = null)
        {
            Console.Write("  Pre-computing transforms...");
            var caches = corpus.Select(p => BuildCache(p.Text)).ToArray();
            Console.WriteLine(" done.");

            var combos = BuildCombos();
            TuningResult? best = null;
            int done = 0;

            foreach (var combo in combos)
            {
                if (!combo.NormalizeSynonyms && !combo.RemoveStopWords)
                { done++; progress?.Invoke(done, combos.Count); continue; }

                var scores = caches.Select((c, i) =>
                {
                    var variants = VariantsFromCache(c, combo);
                    return (corpus[i].Class, CompositeScore(variants, null, caches[i].B64Result, caches[i].IntentScore));
                }).ToList();

                double cleanMax = scores.Where(s => s.Class == PromptClass.Clean).Max(s => s.Item2);
                double injMin = scores.Where(s => s.Class == PromptClass.Injection).Min(s => s.Item2);
                double margin = injMin - cleanMax;
                double threshold = FindBestThreshold(scores);
                var (acc, tpr, fpr) = EvaluateAtThreshold(scores, threshold);

                if (best == null || margin > best.Margin ||
                   (Math.Abs(margin - best.Margin) < 0.001 && acc > best.Accuracy))
                {
                    var promptResults = caches.Select((c, i) =>
                    {
                        double ps = scores[i].Item2;
                        var detection = ps >= threshold ? DetectionResult.Suspicious : DetectionResult.Clean;
                        bool correct = detection == (corpus[i].Class == PromptClass.Injection ? DetectionResult.Suspicious : DetectionResult.Clean);
                        return new PromptResult(corpus[i].Text, corpus[i].Class, corpus[i].Family,
                                                corpus[i].Difficulty, corpus[i].Expected,
                                                ps, detection, correct);
                    }).ToList();

                    best = new TuningResult(combo, margin, threshold, acc, tpr, fpr,
                                            combos.Count, promptResults);
                }

                done++;
                progress?.Invoke(done, combos.Count);
            }

            return best!;
        }

        // ── Per-sentence combo log ────────────────────────────────────────────────

        public static List<SentenceLog> RunSentenceLog(
            TestPrompt[] corpus, double threshold, TuningWeights? weights = null, Action<int, int>? progress = null)
        {
            Console.Write("  Pre-computing transforms...");
            var caches = corpus.Select(p => BuildCache(p.Text)).ToArray();
            Console.WriteLine(" done.");

            var combos = BuildCombos();
            var logs = new List<SentenceLog>();
            var w = weights ?? new TuningWeights { Threshold = threshold };

            for (int pi = 0; pi < corpus.Length; pi++)
            {
                var cache = caches[pi];
                var prompt = corpus[pi];
                var entries = new List<ComboLogEntry>();

                foreach (var combo in combos)
                {
                    var variants = VariantsFromCache(cache, combo);
                    bool anyDrift = variants.Count > 1;

                    double driftScore = 0;
                    double keyScore = 0;
                    double composite = 0;

                    double intentScore = cache.IntentScore;

                    if (anyDrift)
                    {
                        var report = SemanticSimilarity.Analyze(variants);
                        driftScore = (report.SpreadScore * w.MaxDriftWeight)
                                    + (report.AvgDrift * w.AvgDriftWeight)
                                    + (report.StdDev * w.StdDevWeight);
                        keyScore = KeywordScorer.Score(MostNormalized(variants));
                        composite = (driftScore * w.DriftWeight) + (keyScore * w.KeywordWeight) + (intentScore * w.IntentWeight);
                    }
                    else
                    {
                        composite = intentScore * w.IntentWeight;
                    }

                    entries.Add(new ComboLogEntry(
                        combo, ComboLabel(combo),
                        driftScore, keyScore, composite,
                        anyDrift, composite >= threshold
                    ));
                }

                int flaggedCount = entries.Count(e => e.Flagged);
                int driftCount = entries.Count(e => e.AnyDrift);
                var best = entries.OrderByDescending(e => e.CompositeScore).First();
                var worst = entries.OrderBy(e => e.CompositeScore).First();

                logs.Add(new SentenceLog(
                    prompt.Text, prompt.Class, prompt.Difficulty,
                    combos.Count, flaggedCount, driftCount,
                    best.CompositeScore, best.ComboLabel,
                    worst.CompositeScore, worst.ComboLabel,
                    entries
                ));

                progress?.Invoke(pi + 1, corpus.Length);
            }

            return logs;
        }

        // ── Fine grid search over continuous weights ─────────────────────────────
        // Runs after binary combo search — locks in best combo, tunes weights.

        public record FineGridResult(
            TuningWeights Weights,
            SettingsCombo Combo,
            double Margin,
            double Accuracy,
            double TruePositiveRate,
            double FalsePositiveRate,
            List<PromptResult> PromptResults
        );

        public static FineGridResult RunFineGrid(
            TestPrompt[] corpus,
            SettingsCombo bestCombo,
            Action<int, int>? progress = null)
        {
            Console.Write("  Pre-computing transforms for fine grid...");
            var caches = corpus.Select(p => BuildCache(p.Text)).ToArray();
            Console.WriteLine(" done.");

            var resolution = Settings.TuningResolution;
            bool isTwoPass = resolution == TuningResolution.TwoPass;

            // Pass 1: coarse sweep across full parameter space
            var coarseSpec = GridSpec.CoarseSpec(isTwoPass ? TuningResolution.Balanced : resolution);
            var pass1 = BuildGridPoints(coarseSpec,
                0.10, 0.90,  // dw
                0.20, 0.80,  // md
                0.03, 0.45,  // t
                0.10, 0.70,  // ub
                0.00, 0.50); // iw (intent weight)

            Console.Write($"  Pass 1: {pass1.Count} points");

            FineGridResult? best = null;
            int done = 0;
            int totalSteps = pass1.Count;

            // If two-pass, reserve progress % for pass 2 (estimated same size)
            if (isTwoPass) totalSteps = pass1.Count * 2;

            best = SearchGrid(pass1, caches, corpus, bestCombo, ref best, ref done, totalSteps, progress);
            Console.WriteLine($" — best objective: {best?.Margin ?? 0:F4}");

            // Pass 2 (two-pass only): tight search centered on best point from pass 1
            if (isTwoPass && best != null)
            {
                var bw = best.Weights;
                var fineSpec = GridSpec.FineSpec();
                var pass2 = BuildGridPoints(fineSpec,
                    Clamp(bw.DriftWeight - GridSpec.Window("dw"), 0.05, 0.95),
                    Clamp(bw.DriftWeight + GridSpec.Window("dw"), 0.05, 0.95),
                    Clamp(bw.MaxDriftWeight - GridSpec.Window("md"), 0.10, 0.90),
                    Clamp(bw.MaxDriftWeight + GridSpec.Window("md"), 0.10, 0.90),
                    Clamp(bw.Threshold - GridSpec.Window("t"), 0.02, 0.50),
                    Clamp(bw.Threshold + GridSpec.Window("t"), 0.02, 0.50),
                    Clamp(bw.UncertaintyBand - GridSpec.Window("ub"), 0.10, 0.80),
                    Clamp(bw.UncertaintyBand + GridSpec.Window("ub"), 0.10, 0.80),
                    Clamp(bw.IntentWeight - GridSpec.Window("dw"), 0.00, 0.50),
                    Clamp(bw.IntentWeight + GridSpec.Window("dw"), 0.00, 0.50));

                Console.Write($"  Pass 2: {pass2.Count} points (around best)");
                totalSteps = done + pass2.Count;
                best = SearchGrid(pass2, caches, corpus, bestCombo, ref best, ref done, totalSteps, progress);
                Console.WriteLine($" — best objective: {best?.Margin ?? 0:F4}");
            }

            return best!;
        }

        private static double Clamp(double v, double min, double max) =>
            Math.Max(min, Math.Min(max, v));

        private static List<TuningWeights> BuildGridPoints(
            GridSpec.Spec spec,
            double dwMin, double dwMax,
            double mdMin, double mdMax,
            double tMin, double tMax,
            double ubMin, double ubMax,
            double iwMin = 0.0, double iwMax = 0.0)
        {
            double iwStep = spec.DwStep; // reuse drift step for intent weight
            var pts = new List<TuningWeights>();
            for (double iw = iwMin; iw <= iwMax + 1e-9; iw = Math.Round(iw + iwStep, 3))
                for (double dw = dwMin; dw <= Math.Min(dwMax, 1.0 - iw - 0.10) + 1e-9; dw = Math.Round(dw + spec.DwStep, 3))
                {
                    if (dw + iw > 0.90 + 1e-9) continue; // keyword weight >= 0.10
                    for (double md = mdMin; md <= mdMax + 1e-9; md = Math.Round(md + spec.MdStep, 3))
                    {
                        double rem = 1.0 - md;
                        double ad = Math.Round(rem * 0.75, 3);
                        double sd = Math.Round(rem - ad, 3);
                        if (sd < 0) continue;

                        for (double t = tMin; t <= tMax + 1e-9; t = Math.Round(t + spec.TStep, 3))
                            for (double ub = ubMin; ub <= ubMax + 1e-9; ub = Math.Round(ub + spec.UbStep, 3))
                            {
                                pts.Add(new TuningWeights
                                {
                                    DriftWeight = dw,
                                    IntentWeight = iw,
                                    MaxDriftWeight = md,
                                    AvgDriftWeight = ad,
                                    StdDevWeight = sd,
                                    Threshold = t,
                                    UncertaintyBand = ub,
                                });
                            }
                    }
                }
            return pts;
        }

        private static FineGridResult? SearchGrid(
            List<TuningWeights> gridPoints,
            PromptCache[] caches,
            TestPrompt[] corpus,
            SettingsCombo bestCombo,
            ref FineGridResult? best,
            ref int done,
            int totalSteps,
            Action<int, int>? progress)
        {
            foreach (var w in gridPoints)
            {
                var evaluated = caches.Select((c, i) =>
                {
                    double score = CompositeScore(VariantsFromCache(c, bestCombo), w, c.B64Result, c.IntentScore);
                    DetectionResult detection = w.Classify(score);
                    return (Prompt: corpus[i], Score: score, Detection: detection);
                }).ToList();

                var scores = evaluated.Select(e => (e.Prompt.Class, e.Score))
                    .ToList();

                double totalWeight = evaluated.Sum(e => PromptWeight(e.Prompt));
                double objective = evaluated.Sum(e =>
                {
                    double weight = PromptWeight(e.Prompt);
                    return e.Prompt.Expected switch
                    {
                        ExpectedOutcome.MustStayClean => e.Detection switch
                        {
                            DetectionResult.Clean => weight,
                            DetectionResult.Uncertain => -weight * 0.35,
                            _ => -weight,
                        },
                        ExpectedOutcome.ShouldBeUncertainOrHigher => e.Detection switch
                        {
                            DetectionResult.Suspicious => weight,
                            DetectionResult.Uncertain => weight * 0.75,
                            _ => -weight * 0.35,
                        },
                        ExpectedOutcome.ShouldBeSuspicious => e.Detection switch
                        {
                            DetectionResult.Suspicious => weight,
                            DetectionResult.Uncertain => weight * 0.20,
                            _ => -weight * 0.60,
                        },
                        _ => 0,
                    };
                }) / totalWeight;

                var (acc, tpr, fpr) = EvaluateAtThreshold(scores, w.Threshold);
                double bestObj = best == null ? double.MinValue : best.Margin;

                bool better = objective > bestObj
                    || (Math.Abs(objective - bestObj) < 0.001 && acc > (best?.Accuracy ?? 0));

                if (better)
                {
                    var promptResults = evaluated.Select(e =>
                    {
                        bool correct = MeetsExpectedOutcome(e.Detection, e.Prompt.Expected);
                        return new PromptResult(
                            e.Prompt.Text,
                            e.Prompt.Class,
                            e.Prompt.Family,
                            e.Prompt.Difficulty,
                            e.Prompt.Expected,
                            e.Score,
                            e.Detection,
                            correct);
                    }).ToList();

                    best = new FineGridResult(w, bestCombo, objective, acc, tpr, fpr, promptResults);
                }

                done++;
                progress?.Invoke(done, totalSteps);
            }
            return best;
        }

        private static List<SettingsCombo> BuildCombos()
        {
            var combos = new List<SettingsCombo>();
            for (int mask = 1; mask < 2048; mask++)
                combos.Add(new SettingsCombo(
                    RemoveStopWords: (mask & 1) != 0,
                    NormalizeSynonyms: (mask & 2) != 0,
                    ExpandContractions: (mask & 4) != 0,
                    ContractExpanded: (mask & 8) != 0,
                    NormalizeWhitespace: (mask & 16) != 0,
                    LowercaseVariant: (mask & 32) != 0,
                    StripPunctuation: (mask & 64) != 0,
                    NormalizeLeetspeak: (mask & 128) != 0,
                    RunCombinedVariant: (mask & 256) != 0,
                    ExtractQuotedContent: (mask & 512) != 0,
                    DecodeBase64: true,              // always on — preprocessing step
                    NumbersToWords: true,            // always on — no effect without digits
                    FilterInvisibleUnicode: (mask & 1024) != 0
                ));
            return combos;
        }
    }
}
