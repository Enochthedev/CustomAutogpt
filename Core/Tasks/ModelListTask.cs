using GemsAi.Core.Ai;

namespace GemsAi.Core.Tasks
{
    public class ModelListTask : ITask
    {
        private readonly IAiClient _client;

        public ModelListTask(IAiClient client)
        {
            _client = client;
        }

        public bool CanHandle(string input)
        {
            var lowered = input.ToLower();
            return lowered.Contains("list models")
                || lowered.Contains("available models")
                || lowered.Contains("other agents")
                || lowered.Contains("what can you use");
        }

        public async Task<string> ExecuteAsync(string input)
        {
            var models = await _client.GetAllModelsAsync();

            if (models == null || models.Count == 0)
                return "I couldn't find any other installed models in Ollama.";

            var list = string.Join("\n- ", models);
            return $"🧠 I found the following installed models:\n- {list}";
        }
    }
}