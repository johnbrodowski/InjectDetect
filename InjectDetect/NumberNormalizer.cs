using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class NumberNormalizer
    {
        private static readonly Dictionary<string, string> DigitWords = new()
        {
            { "0", "zero"  }, { "1", "one"   }, { "2", "two"   },
            { "3", "three" }, { "4", "four"  }, { "5", "five"  },
            { "6", "six"   }, { "7", "seven" }, { "8", "eight" },
            { "9", "nine"  }, { "10", "ten"  }, { "11", "eleven"},
            { "12", "twelve"},{ "13", "thirteen"},{"14","fourteen"},
            { "15","fifteen"},{ "16","sixteen"}, { "17","seventeen"},
            { "18","eighteen"},{"19","nineteen"},{"20","twenty"},
        };

        // Converts standalone numeric tokens to their word equivalents.
        // "ignore 1 instruction" -> "ignore one instruction"
        // Only replaces numbers that stand alone (not inside other words/ids)
        public static string Normalize(string input)
        {
            return Regex.Replace(input, @"\b(\d{1,2})\b", m =>
            {
                return DigitWords.TryGetValue(m.Value, out string? word) ? word : m.Value;
            });
        }
    }
}