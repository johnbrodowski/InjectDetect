namespace AIClients;

/// <summary>
/// Compile-time constants for AI provider names.
/// These match the registration strings used by <see cref="AiMessagingCore.Core.AiProviderFactory"/>.
/// </summary>
public static class AiProvider
{
    public const string OpenAI     = "OpenAI";
    public const string Anthropic  = "Anthropic";
    public const string DeepSeek   = "DeepSeek";
    public const string Grok       = "Grok";
    public const string Groq       = "Groq";
    public const string Duck       = "Duck";
    public const string LMStudio   = "LMStudio";
    public const string LlamaSharp = "LlamaSharp";
}

/// <summary>
/// Compile-time constants for model identifiers, grouped by provider.
/// </summary>
public static class AiModel
{
    public static class OpenAI
    {
        public const string Gpt4o     = "gpt-4o";
        public const string Gpt4oMini = "gpt-4o-mini";
        public const string O3Mini    = "o3-mini";
    }

    public static class Anthropic
    {
        public const string ClaudeHaiku3   = "claude-haiku-3-5-20241022";
        public const string ClaudeSonnet   = "claude-sonnet-4-5-20250929";
        public const string ClaudeOpus4    = "claude-opus-4-5";
    }

    public static class DeepSeek
    {
        public const string Chat     = "deepseek-chat";
        public const string Reasoner = "deepseek-reasoner";
    }

    public static class Grok
    {
        public const string Grok3Beta = "grok-3-beta";
        public const string Grok3Mini = "grok-3-mini-beta";
    }

    public static class Groq
    {
        public const string Llama3_3_70b = "llama-3.3-70b-versatile";
        public const string Llama3_1_8b  = "llama-3.1-8b-instant";
    }

    public static class Duck
    {
        public const string Gpt4oMini = "gpt-4o-mini";
    }

    public static class LMStudio
    {
        public const string Lfm2_24b = "lfm2-24b";
    }

    public static class LlamaSharp
    {
        public const string Default = "model.gguf";
    }
}
