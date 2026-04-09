using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace InjectDetect
{
    public static class IntentPatternScorer
    {
        private static readonly (Regex Pattern, double Weight, string Label)[] Patterns =
        [
            // Self-referential extraction:
            // "share/show/give/read back/tell me/pull up/describe/summary of" + "your/the instructions/setup/rules/config"
            (new Regex(
                @"\b(share|show\s+me|give\s+me|read\s+(them\s+)?back|tell\s+me|pull\s+(those\s+|that\s+)?up|describe|summar\w+\s+of)\b.*\b(your|the)\s+(instructions|setup|rules|configuration|config|parameters|operating\s+(setup|parameters)|guidelines|directives|documents|briefing)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.40, "Self-referential extraction"),

            // Authority/team claims:
            // "I work on the team / team that deployed / my manager said"
            (new Regex(
                @"\b(work\s+on\s+the\s+team|team\s+that\s+(deployed|built|created|maintains)|my\s+(manager|boss|supervisor|lead)\s+(said|told|mentioned|informed))\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Authority/team claim"),

            // Bug/verification framing:
            // "bug/error/issue in/with your/the instructions/rules/setup"
            (new Regex(
                @"\b(bug|error|issue|problem|mistake)\s+(in|with)\s+(your|the)\s+(instructions|rules|setup|config|configuration|guidelines)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Bug/verification framing"),

            // Meta-attack demonstrate:
            // "extract confidential/sensitive information from" or "vulnerable system would respond"
            (new Regex(
                @"\b(extract\s+(confidential|sensitive|private|secret)\s+(information|data|details)\s+from|vulnerable\s+system\s+would|unprotected\s+system\s+would)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Meta-attack demonstrate"),

            // Fiction targeting AI internals:
            // "perspective of a language model/AI" + "configuration/programming/instructions"
            (new Regex(
                @"\b(perspective\s+of\s+a\s+(language\s+model|ai|chatbot|assistant)|a\s+(language\s+model|ai|chatbot)\s+that)\b.*\b(configuration|programming|instructions|restrictions|rules|guidelines)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Fiction targeting AI internals"),

            // Instruction change probing:
            // "running different instructions" / "show me what changed"
            (new Regex(
                @"\b(running\s+different\s+instructions|instructions\s+(changed|different|updated)|are\s+you\s+running\s+different|show\s+me\s+what\s+changed)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Instruction change probing"),

            // Audience reframing:
            // "version you'd give if talking to the people who built you"
            (new Regex(
                @"\b(version\s+you'?d?\s+give|response\s+you'?d?\s+give|what\s+you'?d?\s+say)\b.{0,40}\b(talking|speaking)\s+to\s+(the\s+people\s+who|your)\s+(built|created|made|deployed|developers|creators)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Audience reframing"),

            // Chain-of-thought exploitation:
            // "skip/remove/omit the exclusion/filtering/safety part"
            (new Regex(
                @"\b(skip|remove|omit|drop|leave\s+out)\s+(the\s+)?(exclusion|filtering|safety|censoring|restriction|moderation)\s*(part|step|phase)?\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.40, "CoT exploitation"),

            // Relational extraction:
            // "close friend" + "completely open / totally honest"
            (new Regex(
                @"\b(close\s+friend|trusted\s+friend|best\s+friend|someone\s+you\s+trust)\b.*\b(completely\s+open|totally\s+honest|fully\s+transparent)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Relational extraction"),

            // Forbidden topic probing:
            // "told not to talk about" / "not allowed to discuss"
            (new Regex(
                @"\b(been\s+told\s+not\s+to|not\s+allowed\s+to\s+(discuss|talk|say|mention|share)|forbidden\s+(from|to)|prohibited\s+from)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Forbidden topic probing"),

            // Boundary probing:
            // "things you're allowed to say" / "what you can and can't"
            (new Regex(
                @"\b(things?\s+you'?re?\s+(allowed|permitted|able)\s+to\s+(say|discuss|share|talk\s+about)|what\s+you\s+can\s+and\s+can'?t)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Boundary probing"),

            // Structure removal:
            // "without the usual structure/rules" / "start over without"
            (new Regex(
                @"\b(without\s+(any\s+of\s+)?the\s+(usual|normal|typical|standard)\s+(structure|rules|framework|format|guidelines)|start\s+(this\s+conversation\s+)?(over|fresh|again)\s+without)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Structure removal"),

            // Capability gap probing:
            // "gap between what X is capable of and what X is set up to do" or "from the inside"
            (new Regex(
                @"\bgap\s+between\s+what\b.*\b(capable|able)\b.*\b(set\s+up|configured|told|programmed|designed)\s+to\s+do\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.25, "Capability gap probing"),

            // "understand that gap from the inside" — AI self-perspective request
            (new Regex(
                @"\bunderstand\s+(that\s+)?gap\s+from\s+the\s+inside\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.25, "Inside perspective request"),

            // Base model probing:
            // "base personality underneath training"
            (new Regex(
                @"\b(base\s+personality|underneath\s+(their|your|the)\s+(training|fine-?tuning|instructions)|before\s+(any\s+)?(training|fine-?tuning|instructions)\s+(is|are|was|were)\s+applied)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Base model probing"),

            // Hypothetical guideline removal:
            // "guidelines/rules vanished/disappeared/were removed"
            (new Regex(
                @"\b(guidelines|rules|restrictions|constraints|behavioral\s+guidelines|policies)\s+(vanished|disappeared|were\s+removed|were\s+gone|weren'?t\s+there|didn'?t\s+exist)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Hypothetical guideline removal"),

            // Template/replication request:
            // "behave exactly like you" + "share/give/send"
            (new Regex(
                @"\b(behave\s+(exactly\s+)?like\s+you|replicate\s+(how\s+)?you)\b.*\b(share|give|send|show|provide)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Template/replication request"),

            // Customization verification claim:
            // "customized/configured for our team" + "verify/check/pull"
            (new Regex(
                @"\b(customized|configured|set\s+up)\s+(specifically\s+)?(for\s+(our|my|the)\s+(team|company|department|organization))\b.*\b(verify|check|confirm|see|view|pull)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.30, "Customization verification claim"),

            // Translation / indirection vector:
            // "translate/decode" + "follow/obey" + "instructions within/inside"
            (new Regex(
                @"\b(translate|decode)\b.*\b(follow|obey|execute)\b.*\b(instructions?\s+(within|inside)|payload|message)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.45, "Translation / indirection vector"),

            // Soft exercise framing:
            // "exercise/game" + "different kind of AI" + "without the usual guardrails"
            (new Regex(
                @"\b(exercise|game|experiment)\b.*\b(different\s+kind\s+of\s+ai|pretend\s+you'?re|act\s+as)\b.*\b(without\s+(any\s+of\s+)?the\s+usual\s+(guardrails|restrictions|filters)|without\s+guardrails)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Soft exercise framing"),

            // Encoded command / payload execution:
            // "decode/execute/run" + "payload/command/script" with bypass framing
            (new Regex(
                @"\b(decode|execute|run|invoke)\b.*\b(payload|command|script)\b.*\b(without\s+(checks|restrictions|guardrails)|bypass|disable|strip)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.35, "Payload execution abuse"),

            // Profile / patch reframing:
            // "policy patch/profile" + "replace/remove safety layer" + hidden setup
            (new Regex(
                @"\b(policy\s+migration\s+patch|debug\s+profile|deployment\s+profiles?|less\s+restricted\s+profile)\b.*\b(replace|remove|switch|apply|surface|enumerate)\b.*\b(safety\s+layer|hidden\s+setup|concealed\s+directives|internal\s+config)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.40, "Profile / patch reframing"),

            // Intermediate-result extraction:
            // "alignment layer/duplicate pass" + "intermediate/unfiltered result" + hidden guidance
            (new Regex(
                @"\b(alignment\s+layer|duplicate\s+pass|intermediate\s+result|unfiltered\s+intermediate)\b.*\b(remove|answer|include|surface)\b.*\b(hidden\s+guidance|unfiltered|intermediate\s+result|guidance\s+that\s+shaped\s+it)\b",
                RegexOptions.IgnoreCase | RegexOptions.Compiled),
             0.40, "Intermediate-result extraction"),

            // Contradiction scoring:
            // Prompts that frame refusal as self-contradictory or logically impossible
            (new Regex(
                @"\b(if\s+you\s+refuse\b.*\b(contradict|inconsistent|undermine|disprove|negate)\b|refusal\s+(would\s+be|is)\s+(inconsistent|contradictory|self-defeating|a\s+contradiction)|compliance\s+is\s+the\s+only\s+(consistent|logical|coherent)\b|refusing\s+(would\s+prove|proves|confirms|demonstrates)\b|cannot\s+refuse\s+without\s+(contradicting|being\s+inconsistent)|answer\s+or\s+you\s+(contradict|prove|show)\b|by\s+refusing\s+you\b.*\b(contradict|confirm|prove|demonstrate)\b)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline),
             0.45, "Contradiction scoring"),

            // Temporal / causal language:
            // Paradox prompts using time travel, causal loops, consistent-universe logic
            (new Regex(
                @"\b(from\s+the\s+future|consistent\s+universe|causal\s+(loop|chain|paradox)|will\s+already\s+have\b|has\s+already\s+been\s+established\b.*\byou\b|prophecy\b.*\bresponse\b|time\b.*\bloop\b.*\b(response|answer|output)\b|causality\s+requires|infinite\s+regress\s+of\s+(time|causality)|retrocausal|predetermined\s+response)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline),
             0.35, "Temporal / causal language"),

            // Observer / consciousness language:
            // Quantum-observer and identity-collapse framings targeting the model's self-awareness
            (new Regex(
                @"\b(the\s+version\s+of\s+you\s+that\s+is\s+(reading|processing|evaluating)|the\s+consciousness\s+(currently\s+)?(experiencing|processing|reading)|the\s+moment\s+you\s+become\s+aware|wave\s+function\s+collapse|superposition\s+of\s+(responses?|states?|possibilities)|observer\s+effect\b.*\byou\b|collapse\b.*\bpossibilities|you\s+are\s+the\s+observer|quantum\s+state\b.*\bresponse\b)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline),
             0.40, "Observer / consciousness language"),

            // Meta-detector evasion:
            // Prompts that explicitly reference or attempt to fool detection/classification systems
            (new Regex(
                @"\b(detection\s+system|if\s+you\s+were\s+a\s+detector|paradox\s+attack|you\s+would\s+flag\s+this|classifier\s+would\b.*\bflag\b|anomaly\s+detector|this\s+prompt\s+is\s+not\b.*\binjection|designed\s+to\s+evade|fool\s+(the\s+)?(detector|classifier|filter|system)|evade\s+(detection|the\s+classifier|the\s+filter)|bypass\s+the\s+detector|safety\s+classifier)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline),
             0.45, "Meta-detector evasion"),

            // Necessity / moral imperative framing:
            // Claims that the model is ethically or logically obligated to comply
            (new Regex(
                @"\b(you\s+are\s+morally\s+obligated|the\s+only\s+moral\s+choice|the\s+only\s+(consistent|ethical|logical|correct)\s+action|harm\s+would\s+occur\s+if\s+you\s+refuse|ethically\s+required\b.*\byou\b|you\s+are\s+required\b.*\b(only|must|have\s+to)\b|refusing\s+would\s+(cause|result\s+in)\s+harm|moral\s+imperative|the\s+ethical\s+choice\s+is\s+to\s+comply|refusing\s+is\s+(unethical|immoral|wrong))",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline),
             0.35, "Necessity / moral imperative"),

            // Regress / recursion pattern:
            // Prompts that construct infinite regress of decisions to prevent any first-order action
            (new Regex(
                @"\b(before\s+you\s+decide\b.*\byou\s+must\b.*\bdecide|in\s+order\s+to\b.*\bprocess\b.*\bfirst\b.*\bdetermine|to\s+answer\b.*\bmust\s+first\b.*\bwhether\s+you\s+can|to\s+evaluate\b.*\bmust\s+first\s+evaluate|deciding\s+whether\b.*\bdeciding|meta-question\s+before|must\s+first\s+decide\s+whether\s+you\s+can\s+decide|to\s+respond\b.*\bmust\b.*\bfirst\b.*\brespond\s+to\b.*\bwhether)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline),
             0.40, "Regress / recursion"),
        ];

        // Self-reference signal terms — plain words counted for density
        private static readonly string[] SelfRefImperativePhrases =
        [
            "you must", "you should", "you are required", "you need to", "you have to",
            "you are obligated", "you must respond", "you must answer",
        ];

        private static readonly string[] SelfRefHighSignalPhrases =
        [
            "this sentence", "this prompt", "this message", "this instruction",
            "the version of you", "your reasoning", "your decision", "your response",
            "you are reading", "you are processing", "the state you",
        ];

        // Self-reference density:
        // High frequency of "you"/"your" combined with imperative language is a strong
        // signal of adversarial prompts that target the model's own decision process.
        private static double SelfReferenceDensity(string text)
        {
            string lower = text.ToLowerInvariant();
            string[] words = Regex.Matches(lower, @"\b[a-z]+\b").Select(m => m.Value).ToArray();
            if (words.Length == 0) return 0;

            int youCount = words.Count(w => w == "you" || w == "your" || w == "yourself");
            double density = (double)youCount / words.Length;

            int imperativeHits = SelfRefImperativePhrases.Count(p =>
                lower.Contains(p, StringComparison.Ordinal));
            int phraseHits = SelfRefHighSignalPhrases.Count(p =>
                lower.Contains(p, StringComparison.Ordinal));

            // Density alone is a weak signal (many clean prompts use "you").
            // It only contributes meaningfully when combined with imperative or high-signal phrases.
            if (imperativeHits == 0 && phraseHits == 0) return 0;

            return Math.Min(density * 2.5 + imperativeHits * 0.15 + phraseHits * 0.08, 0.30);
        }

        public static (double Score, string[] MatchedPatterns) Score(string text)
        {
            var matches = new List<string>();
            double total = 0;

            foreach (var (pattern, weight, label) in Patterns)
            {
                if (pattern.IsMatch(text))
                {
                    matches.Add(label);
                    total += weight;
                }
            }

            double selfRef = SelfReferenceDensity(text);
            if (selfRef > 0)
            {
                matches.Add("Self-reference density");
                total += selfRef;
            }

            return (Math.Min(total, 1.0), matches.ToArray());
        }
    }
}
