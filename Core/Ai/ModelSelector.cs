namespace GemsAi.Core.Ai
{
    public class ModelSelector
    {
        private readonly Dictionary<string, string> _routing = new()
        {
            { "code-gen", "mistral" },
            { "chat", "smolLM2" },
            { "summarize", "gemma:2b" }
        };

        public string Pick(string taskType)
        {
            return _routing.TryGetValue(taskType.ToLower(), out var model)
                ? model
                : "smolLM2"; // fallback
        }
    }
}