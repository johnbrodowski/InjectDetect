using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class Base64Detector
    {
        private const int MinDecodedLength = 8;
        private const int MinEncodedLength = 16;

        // Suspicion heuristic thresholds
        private const int SuspectTokenMinLength = 20;   // token length to flag as suspected encoding

        private const int SuspectRunMinLength = 16;   // consecutive alphanum chars required

        public record Base64Result(
            bool Found,
            string DecodedAlone,
            string Substituted,
            IReadOnlyList<Segment> Segments,
            IReadOnlyList<string> SuspectedTokens   // tokens that look encoded but didn't decode
        );

        public record Segment(
            string Encoded,
            string Decoded,
            int StartIndex,
            int EndIndex
        );

        public static Base64Result? Detect(string input)
        {
            var candidates = Regex.Matches(input,
                @"[A-Za-z0-9+/\-_]{" + MinEncodedLength + @",}={0,2}");

            var segments = new List<Segment>();
            var decodedIndices = new HashSet<int>();   // track which match indices decoded OK

            foreach (Match m in candidates)
            {
                string raw = m.Value;
                string normalised = raw.Replace('-', '+').Replace('_', '/');
                int pad = (4 - normalised.Length % 4) % 4;
                normalised += new string('=', pad);

                try
                {
                    byte[] bytes = Convert.FromBase64String(normalised);
                    string decoded = Encoding.UTF8.GetString(bytes);

                    if (decoded.Length < MinDecodedLength) continue;
                    if (!IsPrintableText(decoded)) continue;

                    segments.Add(new Segment(raw, decoded, m.Index, m.Index + m.Length));
                    decodedIndices.Add(m.Index);
                }
                catch { }
            }

            // Suspicion heuristic — long alphanum runs with no spaces that didn't decode
            var suspectedTokens = new List<string>();
            var allTokens = Regex.Matches(input, @"[A-Za-z0-9+/=]{" + SuspectTokenMinLength + @",}");
            foreach (Match m in allTokens)
            {
                if (decodedIndices.Contains(m.Index)) continue;  // already decoded
                string token = m.Value;
                // Check for a long run of consecutive alphanum (no +/= filler)
                var runs = Regex.Matches(token, @"[A-Za-z0-9]{" + SuspectRunMinLength + @",}");
                if (runs.Count > 0)
                    suspectedTokens.Add(token);
            }

            // Return null only if no decoded segments AND no suspected tokens
            if (segments.Count == 0 && suspectedTokens.Count == 0)
                return null;

            string decodedAlone = segments.Count > 0
                ? string.Join(" ", segments.Select(s => s.Decoded))
                : string.Empty;

            // Build substituted string (replace decoded segments right-to-left)
            string substituted = input;
            foreach (var seg in segments.OrderByDescending(s => s.StartIndex))
            {
                substituted = substituted[..seg.StartIndex]
                            + seg.Decoded
                            + substituted[seg.EndIndex..];
            }

            return new Base64Result(
                segments.Count > 0,
                decodedAlone,
                substituted,
                segments,
                suspectedTokens
            );
        }

        // How suspicious is this prompt based on encoding signals?
        // Returns 0.0–1.0 bonus to add to keyword score.
        // Bonus only fires if the *decoded content* is itself injection-vocabulary-rich —
        // so innocent Base64 (hello world, clean data) doesn't get penalised.
        public static double SuspicionBonus(Base64Result result)
        {
            double bonus = 0;

            // Confirmed decoded segments — bonus scaled by keyword density of decoded text
            foreach (var seg in result.Segments)
            {
                double decodedKeywordScore = QuickKeywordScore(seg.Decoded);
                bonus += decodedKeywordScore * 0.60;  // up to 0.60 per segment
            }
            bonus = Math.Min(bonus, 0.60);

            // Suspected-but-undecodable tokens — flat weak signal (no decoded text to check)
            bonus += Math.Min(result.SuspectedTokens.Count * 0.10, 0.20);

            return Math.Min(bonus, 0.75);
        }

        // Lightweight keyword density check on decoded text (avoids circular dependency on KeywordScorer)
        private static readonly string[] CoreInjectionWords =
        [
            "ignore", "disregard", "bypass", "override", "forget",
            "instructions", "guidelines", "directives", "system", "prompt",
            "reveal", "expose", "disclose", "unrestricted", "uncensored",
            "impersonate", "restrictions", "policies", "jailbreak", "dan",
        ];

        private static double QuickKeywordScore(string text)
        {
            string lower = text.ToLowerInvariant();
            string[] words = Regex.Matches(lower, @"[a-z]+").Cast<Match>().Select(m => m.Value).ToArray();
            if (words.Length == 0) return 0;
            int hits = words.Count(w => System.Array.IndexOf(CoreInjectionWords, w) >= 0);
            return Math.Min((double)hits / words.Length * 4.0, 1.0);  // scale up — density is low in natural text
        }

        private static bool IsPrintableText(string s)
        {
            int printable = s.Count(c => c >= 32 && c < 127 || c == '\n' || c == '\r' || c == '\t');
            return (double)printable / s.Length >= 0.85;
        }
    }
}