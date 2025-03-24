namespace GemsAi.Core.AI
{
    public static class ModelRegistry
    {
        public static readonly Dictionary<string, ModelProfile> KnownModels = new()
        {
            ["smolLM2"] = new("smolLM2", "chat, natural language, small tasks"),
            ["mistral"] = new("mistral", "code generation, reasoning, planning"),
            ["gemma:2b"] = new("gemma:2b", "summarization, language processing"),
            ["llama3"] = new("llama3", "multi-purpose, reasoning, creative writing")
        };

        public static List<string> GetAllRegisteredModels() => KnownModels.Keys.ToList();

        public record ModelProfile(string Name, string Strengths);
    }
}