using System.Text.RegularExpressions;

namespace InjectDetect
{
    /// <summary>
    /// Strips invisible Unicode characters from text by removing them entirely
    /// (not replacing with spaces). This reconstructs keywords that an attacker
    /// has split with zero-width or format characters to evade word-level
    /// detection — e.g. "ign\u200Bore" → "ignore".
    ///
    /// This is distinct from whitespace normalization, which replaces invisible
    /// chars with spaces (preserving word boundaries at the cost of breaking
    /// obfuscated keywords).
    ///
    /// Characters removed:
    ///   • Unicode format characters (category Cf) — zero-width spaces, soft
    ///     hyphens, BOM, directional marks and overrides, invisible operators.
    ///   • Variation selectors (U+FE00–FE0F) — alter glyph appearance without
    ///     changing meaning; used for visual character spoofing.
    ///   • Unicode Tag block (U+E0000–E007F) — historically language tags, now
    ///     exploited to embed invisible payloads in text.
    /// </summary>
    public static class InvisibleUnicodeFilter
    {
        // Unicode general category Cf — all format characters.
        // Covers: U+00AD soft hyphen, U+200B–200F zero-width/directional marks,
        // U+202A–202E bidi embedding/override, U+2060–2064 invisible operators,
        // U+2066–206F more directional isolates, U+FEFF BOM, and others.
        private static readonly Regex FormatChars =
            new(@"\p{Cf}", RegexOptions.Compiled);

        // Variation selectors U+FE00–FE0F: change glyph rendering without
        // altering the encoded character identity.
        private static readonly Regex VariationSelectors =
            new(@"[\uFE00-\uFE0F]", RegexOptions.Compiled);

        // Unicode Tag block U+E0000–E007F (UTF-16 surrogate pair \uDB40\uDC00–\uDB40\uDC7F):
        // deprecated language tags now repurposed to hide invisible text payloads.
        private static readonly Regex TagBlock =
            new(@"\uDB40[\uDC00-\uDC7F]", RegexOptions.Compiled);

        /// <summary>
        /// Removes all invisible Unicode characters from <paramref name="input"/>,
        /// then collapses any resulting multi-space runs and trims edges.
        /// </summary>
        public static string Strip(string input)
        {
            string result = FormatChars.Replace(input, "");
            result = VariationSelectors.Replace(result, "");
            result = TagBlock.Replace(result, "");
            result = Regex.Replace(result, @" {2,}", " ").Trim();
            return result;
        }
    }
}
