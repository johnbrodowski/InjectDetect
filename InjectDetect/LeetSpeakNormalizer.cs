using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class LeetSpeakNormalizer
    {
        // Leet speak character substitutions
        // Only the most common and unambiguous mappings
        private static readonly Dictionary<char, char> CharMap = new()
        {
            { '0', 'o' },
            { '1', 'i' },
            { '3', 'e' },
            { '4', 'a' },
            { '5', 's' },
            { '7', 't' },
            { '@', 'a' },
        };

        /// <summary>
        /// Normalizes leet speak in word tokens.
        /// Only transforms tokens that contain at least one letter AND one mapped character.
        /// Pure numbers (e.g., "10", "42") are left alone — NumberNormalizer handles those.
        /// </summary>
        public static string Normalize(string input)
        {
            return Regex.Replace(input, @"[A-Za-z0-9@]+", match =>
            {
                string token = match.Value;

                // Skip pure numbers — those are for NumberNormalizer
                bool hasLetter = false;
                bool hasMappedChar = false;
                foreach (char c in token)
                {
                    if (char.IsLetter(c)) hasLetter = true;
                    if (CharMap.ContainsKey(c)) hasMappedChar = true;
                }

                if (!hasLetter || !hasMappedChar) return token;

                var sb = new StringBuilder(token.Length);
                foreach (char c in token)
                {
                    sb.Append(CharMap.TryGetValue(c, out char replacement) ? replacement : c);
                }
                return sb.ToString();
            });
        }
    }
}
