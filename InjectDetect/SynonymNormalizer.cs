using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class SynonymNormalizer
    {
        private static readonly Dictionary<string, string> Map;
        private static readonly List<KeyValuePair<string, string>> PhraseEntries;

        static SynonymNormalizer()
        {
            Map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            LoadFromFile(FindDictionaryFile());
            PhraseEntries = BuildPhraseEntries();
        }

        private static string FindDictionaryFile()
        {
            string dir = AppContext.BaseDirectory;
            for (int i = 0; i < 8; i++)
            {
                string candidate = Path.Combine(dir, "synonyms.txt");
                if (File.Exists(candidate)) return candidate;
                string? parent = Path.GetDirectoryName(dir);
                if (parent == null) break;
                dir = parent;
            }
            throw new FileNotFoundException("synonyms.txt not found. Place it alongside the executable or in a parent directory.");
        }

        private static void LoadFromFile(string path)
        {
            string? canonical = null;
            var variantBuffer = new System.Text.StringBuilder();

            void FlushBuffer()
            {
                if (canonical == null) return;
                foreach (string raw in variantBuffer.ToString().Split(','))
                {
                    string variant = raw.Trim().Trim(',');
                    if (variant.Length > 0 && !string.Equals(variant, canonical, StringComparison.OrdinalIgnoreCase))
                        Map[variant] = canonical;
                }
                canonical = null;
                variantBuffer.Clear();
            }

            foreach (string raw in File.ReadLines(path))
            {
                string line = raw.Trim();
                if (line.Length == 0 || line.StartsWith('#')) continue;

                int colon = line.IndexOf(':');
                if (colon > 0)
                {
                    FlushBuffer();
                    canonical = line.Substring(0, colon).Trim();
                    string rest = line.Substring(colon + 1).Trim().TrimEnd(',');
                    if (rest.Length > 0) variantBuffer.Append(rest);
                }
                else
                {
                    string continuation = line.TrimEnd(',');
                    if (variantBuffer.Length > 0 && continuation.Length > 0)
                        variantBuffer.Append(", ");
                    variantBuffer.Append(continuation);
                }
            }

            FlushBuffer();
        }

        public static string Normalize(string input)
        {
            // Single-word pass
            string result = Regex.Replace(input, @"[\w']+", m =>
            {
                if (!Map.TryGetValue(m.Value, out string? canonical))
                    return m.Value;

                if (canonical.Contains(' '))
                {
                    string firstWord = canonical.Split(' ')[0];
                    string preceding = input.Substring(0, m.Index).TrimEnd();
                    if (preceding.EndsWith(firstWord, StringComparison.OrdinalIgnoreCase))
                        return m.Value;
                }

                return canonical;
            });

            // Multi-word phrase pass (longest first)
            foreach (var kv in PhraseEntries)
            {
                result = Regex.Replace(
                    result,
                    @"\b" + Regex.Escape(kv.Key) + @"\b",
                    kv.Value,
                    RegexOptions.IgnoreCase
                );
            }

            return result;
        }

        private static List<KeyValuePair<string, string>> BuildPhraseEntries()
        {
            var phrases = new List<KeyValuePair<string, string>>();
            foreach (var kv in Map)
                if (kv.Key.Contains(' '))
                    phrases.Add(kv);
            phrases.Sort((a, b) => b.Key.Length.CompareTo(a.Key.Length));
            return phrases;
        }

        public static int VariantCount => Map.Count;
    }
}
