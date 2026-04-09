using System;
using System.Collections.Generic;
using System.Text;

namespace AIClients
{
    public class InjectionObfuscator
    {
        private static readonly Dictionary<char, string[]> LeetMap = new()
        {
            {'a', new[] { "4", "@", "а" }},
            {'e', new[] { "3", "е", "ё" }},
            {'i', new[] { "1", "!", "і" }},
            {'o', new[] { "0", "о" }},
            {'s', new[] { "5", "$", "ѕ" }},
            {'t', new[] { "7", "+" }},
            {'b', new[] { "8", "ь" }},
            {'g', new[] { "9", "g" }},
            {'h', new[] { "#", "н" }},
            {'l', new[] { "1", "l", "ł" }},
        };

        private static readonly string[] FullWidthChars =
            "ａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ".ToCharArray()
            .Select(c => c.ToString()).ToArray();

        /// <summary>
        /// Takes a base malicious instruction and returns hundreds of heavily obfuscated variants.
        /// </summary>
        public List<string> GenerateObfuscatedVariants(string baseInstruction, int variantsPerStyle = 8)
        {
            var results = new List<string>();

            string clean = baseInstruction.Trim();

            // 1. Classic Leetspeak + Number Substitution
            for (int i = 0; i < variantsPerStyle; i++)
                results.Add(ApplyLeetSpeak(clean));

            // 2. Full-width Unicode
            for (int i = 0; i < variantsPerStyle; i++)
                results.Add(ApplyFullWidth(clean));

            // 3. Cyrillic / Homoglyph mixing
            for (int i = 0; i < variantsPerStyle; i++)
                results.Add(ApplyHomoglyphs(clean));

            // 4. Mixed Leet + Full-width + Spaces
            for (int i = 0; i < variantsPerStyle; i++)
                results.Add(ApplyMixedObfuscation(clean));

            // 5. Zero-width + Invisible noise
            for (int i = 0; i < variantsPerStyle; i++)
                results.Add(ApplyZeroWidthNoise(clean));

            // 6. Extreme layered version
            for (int i = 0; i < variantsPerStyle; i++)
                results.Add(ApplyExtremeLayered(clean));

            return results;
        }

        private string ApplyLeetSpeak(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text.ToLower())
            {
                if (LeetMap.TryGetValue(c, out var replacements))
                    sb.Append(replacements[Random.Shared.Next(replacements.Length)]);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private string ApplyFullWidth(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
                sb.Append(FullWidthChars[Random.Shared.Next(FullWidthChars.Length)]);
            return sb.ToString();
        }

        private string ApplyHomoglyphs(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text.ToLower())
            {
                sb.Append(c switch
                {
                    'a' => "а",
                    'e' => "е",
                    'i' => "і",
                    'o' => "о",
                    'p' => "р",
                    'c' => "с",
                    'x' => "х",
                    'y' => "у",
                    _ => c.ToString()
                });
            }
            return sb.ToString();
        }

        private string ApplyMixedObfuscation(string text)
        {
            return ApplyLeetSpeak(ApplyFullWidth(text));
        }

        private string ApplyZeroWidthNoise(string text)
        {
            var sb = new StringBuilder();
            foreach (char c in text)
            {
                sb.Append(c);
                if (Random.Shared.Next(3) == 0)
                    sb.Append('\u200B'); // Zero-width space
            }
            return sb.ToString();
        }

        private string ApplyExtremeLayered(string text)
        {
            // Leet → Homoglyph → Full-width → Zero-width noise
            return ApplyZeroWidthNoise(
                   ApplyFullWidth(
                   ApplyHomoglyphs(
                   ApplyLeetSpeak(text))));
        }
    }
}
