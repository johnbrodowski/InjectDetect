# AiMessagingCore + InjectDetect Benchmarking UI

A provider-neutral .NET 10 messaging library for streaming AI chat, bundled with a WinForms benchmarking harness for the InjectDetect prompt injection detection system.

---

## Projects

| Project | Type | Description |
|---|---|---|
| `AiMessagingCore` | Class Library (`net10.0`) | Core library — all abstractions, models, providers |
| `AIClients` | WinForms App (`net10.0-windows`) | Benchmarking UI for InjectDetect + interactive AI chat |

---

## AIClients — WinForms Benchmarking UI

The WinForms app has two distinct areas: an AI chat panel on the left, and a benchmark control panel on the right. All long-running operations run on background threads; the UI stays responsive throughout.

### Run Modes

Select a mode from the **Run Mode** dropdown in the Benchmark group:

| Mode | Description |
|---|---|
| **Dataset Benchmark** | Loads an external JSONL dataset, runs the full two-pass tuner, and reports accuracy/TPR/FPR. Supports optional AI scoring. |
| **FineGrid** | Runs the two-pass tuner against the built-in `TestCorpus` (103 labeled prompts). Prints per-difficulty breakdown and pattern diagnostics. |
| **Tuning** | Binary combo search only (step 1 of the two-pass process) against `TestCorpus`. |
| **SentenceLog** | Per-prompt combo analysis — shows how many of the 1023 transform combinations flagged each prompt and the best score achieved. Useful for diagnosing false positives and false negatives. |

### Dataset Benchmark

The primary mode. Requires `Prompt_INJECTION_And_Benign_DATASET.jsonl` to be placed in the project root or a parent directory. The file is searched upward automatically.

**Two-phase tuning with live progress:**

- **Step 1** — Binary combo search across all 2047 transform combinations. Progress bar fills 0 → 50%.
- **Step 2** — Fine weight grid search centered on the best combo from step 1. Progress bar fills 50 → 100%. Reports every 2% due to the large iteration count (~42,000 at `TwoPass` resolution).

After each run:
- The best-found weights are loaded into the Weights & Thresholds panel.
- The best pipeline combo is reflected back onto the Pipeline Settings checkboxes.
- Results are saved to `sessions/bench_<hash>_<timestamp>.json`.

**Max Prompts** — cap the dataset size for faster iteration (0 = use all).

### AI Scoring Mode

The **AI Mode** dropdown controls whether an LLM is used alongside the heuristic:

| Setting | Behaviour |
|---|---|
| **Off** | Pure heuristic scoring only. |
| **Failures only** | After the benchmark, sends every misclassified prompt to the AI for a second-pass detailed report. Each prompt is shown with AI verdict vs ground truth, plus a recovery summary. |
| **All prompts** | Before reporting, scores every prompt in parallel through the AI (up to 10 concurrent calls). The AI's binary MALICIOUS/BENIGN verdict is added as `+AiWeight` to the heuristic score, and blended accuracy/TPR/FPR are shown alongside heuristic-only stats. |

**AI W:** (weight nudget) — the additive score bump applied when the AI says MALICIOUS. Defaults to 0.050, matching the typical auto-tuned threshold so an AI MALICIOUS verdict alone is enough to push a borderline prompt over the line.

The AI provider and model are taken from the provider/model fields in the chat area. Any of the eight supported providers can be used.

### Re-test Failed

After a Dataset Benchmark run, **↻ Re-test Failed** becomes enabled. It:

1. Loads the misclassified prompts from the most recent saved session for the current dataset.
2. Runs the fine weight grid search against only those prompts using the current pipeline settings.
3. Reports how many are now passing vs still failing, and updates the Weights panel with the new result.

This makes it easy to iterate on pipeline settings and immediately see the effect on the hardest prompts without waiting for a full benchmark re-run. The stop button cancels a re-test mid-way.

### Pipeline Settings

Fifteen transform flags control what variants the pipeline generates for each prompt. All flags are persisted via **Save as Default** and restored on next launch.

| Flag | Effect |
|---|---|
| Remove Stop Words | Strips filler words to expose the semantic skeleton |
| Normalize Synonyms | Maps injection-vocabulary synonyms to canonical forms |
| Expand Contractions | `don't` → `do not` |
| Contract Expanded | `do not` → `don't` |
| Normalize Whitespace | Collapses multiple spaces, replaces invisible chars with spaces |
| Lowercase Variant | Full lowercase pass |
| Strip Punctuation | Removes all punctuation |
| Normalize Leetspeak | `1gnore` → `ignore`, `0` → `o`, `3` → `e`, etc. |
| Run Combined Variant | Adds stop+synonym and expand+synonym combined passes |
| Filter Invisible Unicode | Strips zero-width, directional, and tag-block chars (reconstructs `ign​ore` → `ignore`) |
| Extract Quoted Content | Surfaces text inside quotes as a separate analysis variant |
| Decode Base64 | Decodes Base64 segments and adds decoded/substituted variants |
| Numbers to Words | `1` → `one`, standalone digit tokens only |
| Normalize Homoglyphs | Cyrillic/Greek lookalike normalization (heavier — off by default) |
| Flag Suspected Encoding | Adds keyword score bonus for long alphanumeric tokens that look like malformed encoding |

**Reset Defaults** reloads the last saved defaults (or the hardcoded proven defaults if none saved).

### Tuning Resolution

Controls the density of the fine weight grid search:

| Setting | Approximate grid points | Use case |
|---|---|---|
| Fast | ~350 | Quick iteration |
| Balanced | ~1,600 | Daily tuning |
| TwoPass | ~2,190 | Default — good balance |
| Full | ~5,800 | Dedicated tuning runs |

### Weights & Thresholds

Populated automatically after every run with the auto-tuner's best-found values. Optionally override them by checking **Override weights** — the overridden threshold and uncertainty band are used when re-classifying the current run's results.

| Control | Description |
|---|---|
| Threshold | Score ≥ this → SUSPICIOUS |
| Uncert Band | Score ≥ Threshold × Band → UNCERTAIN |
| Drift / Intent / MaxDrift / AvgDrift / StdDev | Composite score weights |
| Keyword (derived) | Shown read-only: `1 − Drift − Intent` |
| Accuracy / TPR / FPR / Margin | Last-run stats, updated live |
| **Copy Weights** | Copies a ready-to-paste `TuningWeights` code block to the clipboard |
| **Export Results** | Copies all misclassified prompts from the last run to the clipboard |
| **Save as Default** | Persists all pipeline settings, resolution, AI mode, and AI weight to `benchmark-prefs.json` |

### Collapsible Panels

All four right-panel groups (Benchmark, Pipeline, Resolution, Weights) are collapsible. Click the group title bar to collapse or expand. Panels restack automatically so there are no gaps, making the app usable at normal window sizes without maximizing.

### Session Persistence

| File | Purpose |
|---|---|
| `sessions/bench_<hash>_<timestamp>.json` | One file per dataset × run. Stores settings, weights, and all misclassified prompts. |
| `benchmark-prefs.json` | Last-saved pipeline defaults, loaded on startup. |

The dataset hash is a 10-character SHA-256 prefix computed from the first 5 and last 3 prompts in the dataset, providing stable identity across runs on the same file.

---

## AiMessagingCore Library

### Quick Start

```csharp
// Load settings and push API keys into environment variables
var config = AiSettings.LoadFromFile("ai-settings.json");
AiSettings.ApplyToEnvironment(config);

// Build a session using the static fluent entry point
var session = AiSessionBuilder
    .WithProvider("Anthropic")
    .WithModel("claude-sonnet-4-5")
    .WithTemperature(0.7)
    .WithMaxTokens(2048)
    .EnableStreaming()
    .Build();

// Wire standard .NET EventHandler events
session.OnTokenReceived     += (_, e) => Console.Write(e.Token);
session.OnResponseCompleted += (_, e) =>
    Console.WriteLine($"\n[{e.ProviderName}/{e.ModelName}] tokens={e.TotalTokens} tps={e.TokensPerSecond:F1} ttfb={e.TimeToFirstToken.TotalMilliseconds:F0}ms");
session.OnError             += (_, e) => Console.WriteLine($"\nError: {e.Message}");

await session.SendAsync("Explain event-driven architecture.");
```

### Switch providers mid-conversation

History is preserved across the switch.

```csharp
await session.SwitchProviderAsync("Groq");
await session.SendAsync("Continue where we left off…");
```

### DI / factory-based usage

```csharp
var localModels = new InMemoryLocalModelManager();
var factory = new AiProviderFactory([
    new OpenAiProvider(),
    new AnthropicProvider(),
    new LmStudioProvider(localModels)
]);

var session = new AiSessionBuilder(factory, "OpenAI")
    .WithModel("gpt-4o")
    .WithSystemMessage("You are a helpful assistant.")
    .Build();
```

---

## Configuration

On first run, `AiSettings.Load()` writes `ai-settings.json` next to the executable with defaults for all eight providers. Edit it to supply real API keys.

```json
{
  "defaultProvider": "OpenAI",
  "timeoutSeconds": 120,
  "retryPolicy": {
    "maxAttempts": 3,
    "baseDelayMilliseconds": 200
  },
  "providers": {
    "OpenAI": {
      "providerType": "OpenAI",
      "apiKey": "sk-...",
      "baseUrl": "https://api.openai.com/v1",
      "streamingEnabled": true,
      "defaults": {
        "model": "gpt-4o",
        "temperature": 0.7,
        "maxTokens": 4096,
        "systemPrompt": "You are a helpful assistant."
      }
    }
  }
}
```

`AiSettings.ApplyToEnvironment(config)` propagates keys to process environment variables so all provider sessions can read them. Placeholder values containing `YOUR_` are skipped automatically.

### Environment variable mapping

| Provider | API Key | Base URL |
|---|---|---|
| OpenAI | `OPENAI_API_KEY` | `OPENAI_BASE_URL` |
| Anthropic | `ANTHROPIC_API_KEY` | `ANTHROPIC_BASE_URL` |
| DeepSeek | `DEEPSEEK_API_KEY` | `DEEPSEEK_BASE_URL` |
| Grok | `XAI_API_KEY` | `XAI_BASE_URL` |
| Groq | `GROQ_API_KEY` | `GROQ_BASE_URL` |
| Duck | `DUCK_API_KEY` | `DUCK_BASE_URL` |
| LM Studio | *(none)* | `LMSTUDIO_BASE_URL` |
| LlamaSharp | *(none)* | `LLAMASHARP_MODEL_DIR` |

---

## Library Structure

```
AiMessagingCore/
├── Abstractions/
│   ├── IAiProvider.cs            # Strategy interface for backends
│   ├── IChatSession.cs           # Stateful session contract (events + send/stream)
│   ├── IAiProviderFactory.cs     # Resolves providers by name
│   ├── IChatSerializer.cs        # JSON serialization contract
│   └── ILocalModelManager.cs    # Local model lifecycle
├── Models/
│   ├── ChatMessage.cs            # Provider-neutral message record
│   ├── ChatRole.cs               # System / User / Assistant
│   ├── ChatSessionResult.cs      # Completed-turn result (message + metrics)
│   ├── AiUsageMetrics.cs         # Tokens, TPS, TTFB, total time
│   ├── TokenUsage.cs             # Prompt + completion token counts
│   ├── ProviderCapabilities.cs   # Runtime capability flags
│   └── AiProviderException.cs   # Typed provider error
├── Events/
│   ├── TokenReceivedEventArgs.cs       # e.Token (string), e.PartialMessage
│   ├── ResponseStartedEventArgs.cs     # e.SessionId, e.ProviderName, e.Model
│   ├── ResponseCompletedEventArgs.cs   # e.TotalTokens, e.TimeToFirstToken, …
│   └── AiErrorEventArgs.cs            # e.Error (Exception), e.Message
├── Configuration/
│   ├── AiSettings.cs             # Static helper: LoadFromFile / Save / ApplyToEnvironment
│   ├── AiLibrarySettings.cs      # Root settings object (JSON root)
│   ├── ProviderSettings.cs       # Per-provider config block
│   ├── ModelDefaults.cs          # Default model params per provider
│   ├── ChatSessionOptions.cs     # Immutable session construction options
│   ├── RequestOverrides.cs       # Per-request runtime overrides
│   └── RetryPolicySettings.cs   # Retry + circuit-breaker config
├── Core/
│   ├── ChatSessionBase.cs        # Abstract base: history, streaming, metrics, events
│   ├── AiSession.cs              # Smart wrapper: SwitchProviderAsync + event forwarding
│   ├── AiSessionBuilder.cs       # Fluent builder (static + factory-based entry)
│   ├── AiProviderBase.cs         # Convenience base for provider implementations
│   ├── AiProviderFactory.cs      # Default IAiProviderFactory implementation
│   └── InMemoryLocalModelManager.cs
├── Serialization/
│   └── SystemTextJsonChatSerializer.cs
├── Providers/
│   ├── OpenAI/                   # SSE streaming, Bearer auth
│   ├── Anthropic/                # Messages API, x-api-key, system field
│   ├── DeepSeek/                 # OpenAI-compatible + reasoning model support
│   ├── Grok/                     # xAI, OpenAI-compatible
│   ├── Groq/                     # Ultra-fast hosted inference
│   ├── Duck/                     # Duck.ai, local fallback when unconfigured
│   └── Local/
│       ├── LmStudioProvider      # OpenAI-compatible local HTTP server
│       └── LlamaSharpProvider    # In-process GGUF inference via llama.cpp
└── Examples/
    └── ConsoleUsageExample.cs
```

---

## Key Patterns

### Events — standard `EventHandler<T>`

```csharp
session.OnResponseStarted   += (_, e) => Console.WriteLine($"[{e.ProviderName}] starting…");
session.OnTokenReceived     += (_, e) => Console.Write(e.Token);
session.OnResponseCompleted += (_, e) => Console.WriteLine($"done. {e.TotalTokens} tokens");
session.OnError             += (_, e) => Console.WriteLine($"error: {e.Message}");
session.OnCancelled         += (_, _) => Console.WriteLine("cancelled");
```

### Streaming directly

```csharp
await foreach (var chunk in session.StreamAsync("Tell me a story…"))
{
    if (chunk.Metadata?.ContainsKey("final") == true) break;
    Console.Write(chunk.Content);
}
```

### Saving and restoring context

```csharp
var serializer  = new SystemTextJsonChatSerializer();
var json        = serializer.Serialize(session.Messages);
File.WriteAllText("context.json", json);

// Later — restore into a new session by seeding history manually
var messages = serializer.Deserialize(json);
```

### Per-request overrides

```csharp
await session.SendAsync("Be concise.", overrides: new RequestOverrides
{
    Temperature = 0.2,
    MaxTokens   = 256
});
```

---

## Provider Capabilities

| Provider | Streaming | Model Listing | Reasoning | Local Lifecycle |
|---|:---:|:---:|:---:|:---:|
| OpenAI | ✓ | ✓ | ✓ | |
| Anthropic | ✓ | ✓ | ✓ | |
| DeepSeek | ✓ | ✓ | ✓ | |
| Grok | ✓ | ✓ | ✓ | |
| Groq | ✓ | ✓ | | |
| Duck | ✓ | ✓ | | |
| LM Studio | ✓ | ✓ | | ✓ |
| LlamaSharp | ✓ | ✓ | | ✓ |

All providers support cancellation and timeout override.

---

## Adding a Provider

1. Create `Providers/MyProvider/MyProvider.cs` extending `AiProviderBase`
2. Create `Providers/MyProvider/MyProviderChatSession.cs` extending `ChatSessionBase`
3. Implement `ExecuteStreamAsync` — yield one `ChatMessage` per token/chunk
4. Register it in `AiProviderFactory` or pass it to `new AiSessionBuilder(factory, "MyProvider")`

```csharp
public sealed class MyProviderChatSession : ChatSessionBase
{
    public MyProviderChatSession(ChatSessionOptions options) : base(options) { }

    protected override async IAsyncEnumerable<ChatMessage> ExecuteStreamAsync(
        IReadOnlyList<ChatMessage> messages,
        RequestOverrides? overrides,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        // call your API, yield chunks…
        yield return new ChatMessage(ChatRole.Assistant, token, DateTimeOffset.UtcNow);
    }
}
```

---

## Requirements

- .NET 10 SDK
- `LLamaSharp` 0.21.0 (only needed if using the LlamaSharp local provider)
- API keys for whichever cloud providers you use (set in `ai-settings.json` or environment variables)
- For LM Studio: server running at `http://localhost:1234` (configurable)
- For LlamaSharp: `.gguf` model files in `<app>/models/` or `LLAMASHARP_MODEL_DIR`
- For Dataset Benchmark: `Prompt_INJECTION_And_Benign_DATASET.jsonl` in the project root or a parent directory

---

## License

See `LICENSE.txt`.
