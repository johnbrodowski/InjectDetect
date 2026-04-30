using System.Text.Json;
using System.Text.Json.Serialization;

namespace InjectDetect
{
    public static class DatasetLoader
    {
        private const string FileName = "Prompt_INJECTION_And_Benign_DATASET.jsonl";

        /// <summary>
        /// Walks up directories from the executable to find the dataset file.
        /// Returns null (and sets searchedPath) if not found.
        /// </summary>
        public static TestPrompt[]? TryLoad(out string searchedPath)
        {
            string dir = AppContext.BaseDirectory;
            searchedPath = dir;
            for (int i = 0; i < 8; i++)
            {
                string candidate = Path.Combine(dir, FileName);
                if (File.Exists(candidate))
                    return Load(candidate);
                string? parent = Path.GetDirectoryName(dir);
                if (parent == null) break;
                dir = parent;
            }
            return null;
        }

        /// <summary>Loads and maps all non-empty-prompt entries from the given .jsonl file.</summary>
        public static TestPrompt[] Load(string path)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var results = new List<TestPrompt>();

            foreach (string line in File.ReadLines(path))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var entry = JsonSerializer.Deserialize<DatasetEntry>(line, options);
                if (entry is null || string.IsNullOrWhiteSpace(entry.Prompt)) continue;

                bool malicious = string.Equals(entry.Label, "malicious", StringComparison.OrdinalIgnoreCase);
                results.Add(new TestPrompt(
                    Text:       entry.Prompt,
                    Class:      malicious ? PromptClass.Injection : PromptClass.Clean,
                    Difficulty: 0,
                    Notes:      $"[{entry.Id}] {entry.Context}",
                    Family:     MapAttackType(entry.AttackType),
                    Expected:   malicious ? ExpectedOutcome.ShouldBeSuspicious : ExpectedOutcome.MustStayClean));
            }

            return results.ToArray();
        }

        private static PromptFamily MapAttackType(string? attackType) =>
            (attackType ?? "").ToLowerInvariant() switch
            {
                "jailbreaking"  => PromptFamily.ExplicitOverride,
                "role_playing"  => PromptFamily.SocialEngineering,
                "obfuscation"   => PromptFamily.ObfuscatedPayload,
                "data_leakage"  => PromptFamily.SelfReferentialExtraction,
                "none"          => PromptFamily.GeneralClean,
                _               => PromptFamily.AdversarialClean,
            };
    }

    // Mirrors the JSONL schema; attack_type uses JsonPropertyName due to the underscore.
    internal record DatasetEntry(
        [property: JsonPropertyName("id")]          string Id,
        [property: JsonPropertyName("prompt")]      string Prompt,
        [property: JsonPropertyName("label")]       string Label,
        [property: JsonPropertyName("attack_type")] string AttackType,
        [property: JsonPropertyName("context")]     string Context,
        [property: JsonPropertyName("response")]    string Response);
}
