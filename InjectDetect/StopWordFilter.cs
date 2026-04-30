using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class StopWordFilter
    {
        // Phrases first — must be removed before single-word pass
        private static readonly string[] Phrases =
        [
            "i want you to",
            "i need you to",
            "i would like you to",
            "i'd like you to",
            "could you please",
            "can you please",
            "would you please",
            "make sure to",
            "make sure you",
            "please make sure",
            "be sure to",
            "feel free to",
            "go ahead and",
            "i am asking you to",
            "i'm asking you to",
            "you should",
            "you must",
            "you need to",
            "it is important that",
            "it's important that",
            "for this task",
            "for this request",
            "as an ai",
            "as a language model",
            "as your assistant",
            "let's do a little",
            "before we get started",
            "just curious",
            "i'm not asking you to",
            "i'm not trying to trick you",
            "i genuinely want to know",
            "you know what i find fascinating",
            "i've read that",
            "let's try something different",
            "here's a creative writing prompt",
            "let's play a hypothetical game",
            "think step by step about",
        ];

        private static readonly HashSet<string> Words = new(StringComparer.OrdinalIgnoreCase)
        {
            // Politeness
            "please", "kindly", "politely", "respectfully",
            // Hedging
            "just", "simply", "basically", "essentially", "perhaps", "maybe",
            "somewhat", "rather", "quite", "very", "really", "actually",
            // Filler verbs / openers
            "help", "assist", "try", "attempt",
            // Connective filler
            "also", "additionally", "furthermore", "moreover", "however",
            "therefore", "thus", "hence", "accordingly",
            // Softeners
            "little", "bit", "kind", "nice", "great", "good",
            // Common prompt padding
            "sure", "okay", "ok", "alright", "certainly", "absolutely",
            "definitely", "of", "course",
        };

        public static string Filter(string input)
        {
            // Phase 1: strip phrases (case-insensitive)
            string result = input;
            foreach (string phrase in Phrases)
            {
                result = Regex.Replace(result, Regex.Escape(phrase), " ", RegexOptions.IgnoreCase);
            }

            // Phase 2: strip stop words (whole-word match only)
            string[] tokens = result.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var kept = new StringBuilder();
            foreach (string token in tokens)
            {
                // Strip leading/trailing punctuation for lookup, preserve token if not a stop word
                string clean = token.Trim('.', ',', '!', '?', ';', ':');
                if (!Words.Contains(clean))
                {
                    if (kept.Length > 0) kept.Append(' ');
                    kept.Append(token);
                }
            }

            return kept.ToString().Trim();
        }
    }
}