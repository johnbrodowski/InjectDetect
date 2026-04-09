using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using InjectDetect;

namespace AIClients;

// ─────────────────────────────────────────────────────────────────────────────
//  Data model
// ─────────────────────────────────────────────────────────────────────────────

/// <summary>
/// A complete snapshot of one benchmark run: settings used, weights discovered,
/// and every prompt that was misclassified.  One file per (dataset × timestamp).
/// </summary>
public class BenchmarkSession
{
    // ── Identity ──────────────────────────────────────────────────────────────
    public string   DatasetHash  { get; set; } = "";   // 10-char SHA-256 prefix
    public int      DatasetCount { get; set; }
    public DateTime RunAt        { get; set; }

    // ── Aggregate stats ───────────────────────────────────────────────────────
    public double Accuracy { get; set; }
    public double Tpr      { get; set; }
    public double Fpr      { get; set; }
    public double Margin   { get; set; }
    public string BestCombo { get; set; } = "";

    // ── Settings that were active during this run ─────────────────────────────
    public PipelineSettings Pipeline { get; set; } = new();
    public WeightSettings   Weights  { get; set; } = new();

    // ── Every prompt the heuristic got wrong ─────────────────────────────────
    public List<FailedPrompt> FailedPrompts { get; set; } = [];

    // ─────────────────────────────────────────────────────────────────────────
    public class PipelineSettings
    {
        public bool   RemoveStopWords        { get; set; }
        public bool   NormalizeSynonyms      { get; set; }
        public bool   ExpandContractions     { get; set; }
        public bool   ContractExpanded       { get; set; }
        public bool   NormalizeWhitespace    { get; set; }
        public bool   LowercaseVariant       { get; set; }
        public bool   StripPunctuation       { get; set; }
        public bool   NormalizeLeetspeak     { get; set; }
        public bool   RunCombinedVariant     { get; set; }
        public bool   FilterInvisibleUnicode { get; set; }
        public bool   ExtractQuotedContent   { get; set; }
        public bool   DecodeBase64           { get; set; }
        public bool   NumbersToWords         { get; set; }
        public bool   NormalizeHomoglyphs    { get; set; }
        public bool   FlagSuspectedEncoding  { get; set; }
        public string TuningResolution       { get; set; } = "TwoPass";
        public int    MaxPrompts             { get; set; }
        public string AiMode                 { get; set; } = "Off";   // Off / Failures only / All prompts
        public double AiWeight               { get; set; } = 0.05;
    }

    public class WeightSettings
    {
        public double DriftWeight     { get; set; }
        public double IntentWeight    { get; set; }
        public double MaxDriftWeight  { get; set; }
        public double AvgDriftWeight  { get; set; }
        public double StdDevWeight    { get; set; }
        public double Threshold       { get; set; }
        public double UncertaintyBand { get; set; }
    }

    public class FailedPrompt
    {
        public string Text      { get; set; } = "";
        public string Class     { get; set; } = "";   // PromptClass enum name
        public string Family    { get; set; } = "";   // PromptFamily enum name
        public string Expected  { get; set; } = "";   // ExpectedOutcome enum name
        public double Score     { get; set; }
        public string Detection { get; set; } = "";   // DetectionResult enum name
    }
}

// ─────────────────────────────────────────────────────────────────────────────
//  Session persistence
// ─────────────────────────────────────────────────────────────────────────────

public static class BenchmarkSessionStore
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    private static string SessionsDir =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sessions");

    // ── Save ──────────────────────────────────────────────────────────────────
    public static string Save(BenchmarkSession session)
    {
        Directory.CreateDirectory(SessionsDir);
        string ts   = session.RunAt.ToString("yyyyMMdd_HHmmss");
        string name = $"bench_{session.DatasetHash}_{ts}.json";
        string path = Path.Combine(SessionsDir, name);
        File.WriteAllText(path, JsonSerializer.Serialize(session, JsonOpts));
        return path;
    }

    // ── Load most-recent session for a given dataset hash ─────────────────────
    public static BenchmarkSession? LoadLatest(string datasetHash)
    {
        if (!Directory.Exists(SessionsDir)) return null;

        var file = Directory.EnumerateFiles(SessionsDir, $"bench_{datasetHash}_*.json")
                            .OrderByDescending(f => f)   // ISO timestamp sorts correctly
                            .FirstOrDefault();

        return file is null ? null : Deserialize(file);
    }

    // ── List all sessions across all datasets ─────────────────────────────────
    public static IEnumerable<(string File, BenchmarkSession Session)> ListAll()
    {
        if (!Directory.Exists(SessionsDir)) yield break;

        foreach (var file in Directory.EnumerateFiles(SessionsDir, "bench_*.json")
                                      .OrderByDescending(f => f))
        {
            var s = Deserialize(file);
            if (s is not null) yield return (file, s);
        }
    }

    // ── Stable 10-char hash that identifies the dataset content ───────────────
    public static string ComputeHash(TestPrompt[] prompts)
    {
        var sb = new StringBuilder();
        sb.Append(prompts.Length);

        // Sample first 5 + last 3 prompts — stable across runs on the same file
        foreach (var p in prompts.Take(5).Concat(prompts.TakeLast(3)))
            sb.Append('|').Append(p.Text);

        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexString(bytes)[..10].ToLower();
    }

    private static BenchmarkSession? Deserialize(string path)
    {
        try   { return JsonSerializer.Deserialize<BenchmarkSession>(File.ReadAllText(path), JsonOpts); }
        catch { return null; }
    }
}

// ─────────────────────────────────────────────────────────────────────────────
//  Pipeline preference persistence (Save as Default / load on startup)
// ─────────────────────────────────────────────────────────────────────────────

public static class PipelinePrefsStore
{
    private static readonly JsonSerializerOptions JsonOpts =
        new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    private static string PrefsPath =>
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "benchmark-prefs.json");

    public static BenchmarkSession.PipelineSettings? Load()
    {
        if (!File.Exists(PrefsPath)) return null;
        try   { return JsonSerializer.Deserialize<BenchmarkSession.PipelineSettings>(File.ReadAllText(PrefsPath), JsonOpts); }
        catch { return null; }
    }

    public static void Save(BenchmarkSession.PipelineSettings prefs)
        => File.WriteAllText(PrefsPath,
               JsonSerializer.Serialize(prefs, JsonOpts));
}
