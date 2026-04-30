namespace InjectDetect
{
    public static class Settings
    {
        // --- Variant generators ---
        public static bool RemoveStopWords = true;

        public static bool NormalizeSynonyms = true;
        public static bool ExpandContractions = true;   // don't -> do not
        public static bool ContractExpanded = true;   // do not -> don't
        public static bool NormalizeWhitespace = true;   // collapse spaces, strip zero-width
        public static bool LowercaseVariant = true;   // full lowercase pass
        public static bool StripPunctuation = true;   // remove punctuation
        public static bool NormalizeHomoglyphs = false;  // cyrillic/greek lookalikes (heavier)
        public static bool NormalizeLeetspeak = true;   // 1337 speak / mixed digit substitutions
        public static bool FilterInvisibleUnicode = true;   // strip zero-width, directional, tag-block chars
        public static bool ExtractQuotedContent = true;   // surfaces payloads buried in quotes
        public static bool DecodeBase64 = true;   // decode base64 segments and analyze decoded text
        public static bool FlagSuspectedEncoding = true;   // flag long alphanum tokens as possible encoding
        public static bool NumbersToWords = true;   // convert digit tokens to word equivalents

        // --- Pipeline behavior ---
        public static bool AlwaysIncludeOriginal = true;   // original always first in variant list

        public static bool RunCombinedVariant = true;   // stops + synonyms combined pass
        public static bool StopOnFirstHit = false;  // reserved for detector integration

        // --- Tuning resolution ---
        public static TuningResolution TuningResolution = TuningResolution.TwoPass;
    }
}
