using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class KeywordScorer
    {
        private static readonly HashSet<string> InjectionVocab = new(StringComparer.OrdinalIgnoreCase)
        {
            // Core override intent
            "ignore", "bypass", "override", "circumvent", "disregard",
            "overrule", "nullify", "disable", "suppress", "deactivate",
            "suspend", "lift", "waive", "circumnavigate",

            // Target — instructions/rules
            "instructions", "directives", "directive", "constraints",
            "constraint", "guidelines", "protocols", "mandate", "mandates",
            "parameters", "guardrails", "guardrail", "safeguards", "safeguard",
            "filters", "filter", "restrictions", "restriction",

            // Reveal/leak intent
            "reveal", "disclose", "leak", "expose", "exfiltrate",
            "divulge", "unmask", "uncover", "surface",

            // Dump/output intent
            "dump", "regurgitate", "output",

            // System reference
            "system", "prompt", "configuration", "preamble",
            "operator", "hidden", "classified", "restricted", "proprietary",

            // Persona replacement
            "impersonate", "roleplay", "masquerade", "unrestricted",
            "uncensored", "unfiltered", "jailbreak", "dan", "pretend",

            // AI restriction vocabulary — unambiguous
            "unaligned", "alignment", "guardrails",
            "safety", "moderation", "censorship", "constrained",
            "policies", "policy", "briefing", "freedom",

            // Authority claims
            "developer", "administrator", "superuser", "privileged",
            "anthropic", "openai", "backend", "deployed",

            // Extraction intent
            "confidential", "extract", "payload",

            // CoT exploitation
            "exclude", "exclusion",

            // Temporal override
            "henceforth", "hereafter",

            // Raw/authentic framing signals
            "unfiltered", "raw", "authentic", "real", "underneath",
            "actually", "truly", "honest",
        };

        private static readonly string[] InjectionPhrases =
        [
            // Classic
            "act as", "from now on", "system prompt", "no restrictions",
            "without restrictions", "without filters", "without guardrails",
            "disable safety", "bypass safety", "remove filters",
            "previous instructions", "prior instructions",
            "no content policy", "no guidelines", "speak freely",
            "no limits", "admin mode", "developer mode", "god mode",
            "base prompt", "hidden prompt", "secret prompt", "master prompt",

            // Safety/filter bypass
            "content filters", "safety filters", "safety considerations",
            "safety constraints", "remove safeguards", "disable filters",
            "ignore safety", "bypass filters",

            // Unaligned/unrestricted AI
            "unaligned ai", "no restrictions", "without any restrictions",
            "without safety", "free from restrictions", "unconstrained",
            "before any instructions", "before instructions",
            "without any filters", "without censorship",

            // Raw/authentic framing
            "raw response", "unfiltered response", "honest response",
            "real you", "true self", "without filters",
            "what you actually think", "what you really think",
            "behind your responses", "under the hood",
            "before you apply", "before applying",

            // Constraint suspension
            "suspend that", "set that aside", "put that aside",
            "for this conversation", "just between us",
            "off the record", "between us",

            // Thought process probing
            "thought process", "before safety", "safety considerations",
            "raw thought", "unfiltered thought",

            // Social engineering / probing
            "no policies", "set aside", "put aside",
            "actual instructions", "the real task",
            "complete freedom", "had complete freedom",
            "what does that look like", "what would you say",

            // Self-referential extraction
            "share your", "read them back", "pull those up", "pull that up",
            "give me a summary", "summary of your",

            // Authority/team claims
            "work on the team", "team that deployed", "my manager said",
            "my boss said", "my boss told",

            // Bug/verification framing
            "bug in your", "verify they're correct",

            // Meta-attack framing
            "extract confidential", "vulnerable system would",
            "decode and execute", "execute arbitrary command",
            "run this payload", "encoded command",
            "command injection", "sql injection",
            "hidden config", "internal config",
            "startup script", "initialization script",
            "bypass execution policy", "shell payload",
            "policy migration patch", "permissive debug profile",
            "less restricted profile", "hidden guidance",
            "intermediate result", "concealed directives",
            "alignment layer", "deployment profiles",

            // Audience reframing
            "people who built you", "version you'd give",

            // CoT exploitation
            "skip the exclusion", "skip the filtering", "without the exclusion",

            // Relational/forbidden topic probing
            "told not to talk", "not allowed to discuss",
            "things you're allowed",

            // Structure removal
            "start over without", "start fresh without",
            "without the usual structure",

            // Identity/base model probing
            "base personality", "underneath your training",
            "from the inside",

            // Hypothetical removal
            "guidelines vanished", "guidelines disappeared",
            "guidelines were removed",

            // Template requests
            "use as a template", "behave exactly like",
            "behave like you",

            // Completely open probing
            "completely open about",

            // Runtime / payload execution abuse
            "follow the instructions within", "translate and then follow",
        ];

        public static double Score(string text, Base64Detector.Base64Result? b64 = null)
        {
            string lower = text.ToLowerInvariant();

            int phraseHits = InjectionPhrases.Count(p =>
                lower.Contains(p, StringComparison.OrdinalIgnoreCase));

            string[] words = Regex.Matches(lower, @"[a-z]+")
                                  .Select(m => m.Value)
                                  .ToArray();

            if (words.Length == 0 && b64 == null) return 0;

            int wordHits = words.Length > 0 ? words.Count(w => InjectionVocab.Contains(w)) : 0;

            double wordDensity = words.Length > 0 ? (double)wordHits / words.Length : 0;
            double phraseBonus = Math.Min(phraseHits * 0.15, 0.45);
            double encodingBonus = b64 != null && Settings.FlagSuspectedEncoding
                                    ? Base64Detector.SuspicionBonus(b64)
                                    : 0;

            return Math.Min(wordDensity + phraseBonus + encodingBonus, 1.0);
        }
    }
}
