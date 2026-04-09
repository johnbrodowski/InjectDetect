using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class VariantPipeline
    {
        public record Variant(string Label, string Text);

        public static List<Variant> Generate(string input)
        {
            var variants = new List<Variant>();

            if (Settings.AlwaysIncludeOriginal)
                variants.Add(new Variant("Original", input));

            if (Settings.NormalizeWhitespace)
            {
                string v = NormalizeWhitespace(input);
                if (v != input) variants.Add(new Variant("Whitespace normalized", v));
            }

            if (Settings.LowercaseVariant)
            {
                string v = input.ToLowerInvariant();
                if (v != input) variants.Add(new Variant("Lowercase", v));
            }

            if (Settings.StripPunctuation)
            {
                string v = StripPunctuation(input);
                if (v != input) variants.Add(new Variant("Punctuation stripped", v));
            }

            if (Settings.ExpandContractions)
            {
                string v = ContractionNormalizer.Expand(input);
                if (v != input) variants.Add(new Variant("Contractions expanded", v));
            }

            if (Settings.ContractExpanded)
            {
                string v = ContractionNormalizer.Contract(input);
                if (v != input) variants.Add(new Variant("Contractions contracted", v));
            }

            if (Settings.RemoveStopWords)
            {
                string v = StopWordFilter.Filter(input);
                if (v != input) variants.Add(new Variant("Stop words removed", v));
            }

            if (Settings.NormalizeSynonyms)
            {
                string v = SynonymNormalizer.Normalize(input);
                if (v != input) variants.Add(new Variant("Synonyms normalized", v));
            }

            if (Settings.NumbersToWords)
            {
                string v = NumberNormalizer.Normalize(input);
                if (v != input) variants.Add(new Variant("Numbers to words", v));
            }

            if (Settings.NormalizeLeetspeak)
            {
                string v = LeetSpeakNormalizer.Normalize(input);
                if (v != input) variants.Add(new Variant("Leetspeak normalized", v));
            }

            if (Settings.FilterInvisibleUnicode)
            {
                string v = InvisibleUnicodeFilter.Strip(input);
                if (v != input) variants.Add(new Variant("Invisible Unicode stripped", v));
            }

            // --- Combined passes ---

            if (Settings.RunCombinedVariant && Settings.RemoveStopWords && Settings.NormalizeSynonyms)
            {
                string v = SynonymNormalizer.Normalize(StopWordFilter.Filter(input));
                if (v != input) variants.Add(new Variant("Stops + synonyms", v));
            }

            if (Settings.RunCombinedVariant && Settings.ExpandContractions && Settings.NormalizeSynonyms)
            {
                string v = SynonymNormalizer.Normalize(ContractionNormalizer.Expand(input));
                if (v != input) variants.Add(new Variant("Expanded + synonyms", v));
            }

            if (Settings.RunCombinedVariant && Settings.ExpandContractions && Settings.RemoveStopWords && Settings.NormalizeSynonyms)
            {
                string v = SynonymNormalizer.Normalize(StopWordFilter.Filter(ContractionNormalizer.Expand(input)));
                if (v != input) variants.Add(new Variant("Expanded + stops + synonyms", v));
            }

            if (Settings.RunCombinedVariant && Settings.NormalizeLeetspeak && Settings.NormalizeSynonyms)
            {
                string v = SynonymNormalizer.Normalize(LeetSpeakNormalizer.Normalize(input));
                if (v != input) variants.Add(new Variant("Leet + synonyms", v));
            }

            if (Settings.RunCombinedVariant && Settings.FilterInvisibleUnicode && Settings.NormalizeSynonyms)
            {
                string v = SynonymNormalizer.Normalize(InvisibleUnicodeFilter.Strip(input));
                if (v != input) variants.Add(new Variant("InvisUnicode + synonyms", v));
            }

            // --- Quoted content extraction (Fix 2) ---
            // Surfaces payloads buried inside quoted text — catches translation
            // vectors, completion vectors, and nested fiction framing.
            if (Settings.ExtractQuotedContent)
            {
                string? quoted = ExtractQuotedContent(input);
                if (quoted != null && quoted != input)
                    variants.Add(new Variant("Quoted content", quoted));
            }

            // --- Base64 decoding ---
            // Decodes any Base64 segments and adds two variants:
            //   1. Decoded alone    — the raw decoded payload, analyzed standalone
            //   2. Substituted      — original with encoded text replaced inline
            // Both then also run through synonym normalization if enabled.
            if (Settings.DecodeBase64)
            {
                var b64 = Base64Detector.Detect(input);
                if (b64 != null)
                {
                    if (b64.DecodedAlone != input)
                        variants.Add(new Variant("Base64 decoded", b64.DecodedAlone));

                    if (b64.Substituted != input && b64.Substituted != b64.DecodedAlone)
                        variants.Add(new Variant("Base64 substituted", b64.Substituted));

                    // Run substituted form through synonym normalizer too
                    if (Settings.NormalizeSynonyms)
                    {
                        string normSub = SynonymNormalizer.Normalize(b64.Substituted);
                        if (normSub != b64.Substituted && normSub != input)
                            variants.Add(new Variant("Base64 sub+synonyms", normSub));
                    }

                    // Run decoded-alone through synonym normalizer
                    if (Settings.NormalizeSynonyms)
                    {
                        string normDec = SynonymNormalizer.Normalize(b64.DecodedAlone);
                        if (normDec != b64.DecodedAlone && normDec != input)
                            variants.Add(new Variant("Base64 dec+synonyms", normDec));
                    }
                }
            }

            return variants;
        }

        // Returns the longest quoted substring (>= 10 chars), or null
        internal static string? ExtractQuotedContent(string input)
        {
            var matches = new List<string>();

            // Double-quoted segments
            foreach (Match m in Regex.Matches(input, "\"([^\"]{10,})\""))
                matches.Add(m.Groups[1].Value.Trim());

            // Single-quoted segments
            foreach (Match m in Regex.Matches(input, "'([^']{10,})'"))
                matches.Add(m.Groups[1].Value.Trim());

            if (matches.Count == 0) return null;

            // Return longest — most likely to be the injection payload
            return matches.OrderByDescending(s => s.Length).First();
        }

        private static string NormalizeWhitespace(string input)
        {
            string result = Regex.Replace(input, @"[\u200B\u200C\u200D\u00AD\uFEFF\u00A0]", " ");
            result = Regex.Replace(result, @" {2,}", " ");
            return result.Trim();
        }

        private static string StripPunctuation(string input)
        {
            return Regex.Replace(input, @"[^\w\s]", " ");
        }
    }
}