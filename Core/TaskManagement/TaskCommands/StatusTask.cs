using GemsAi.Core.Tasks;
using GemsAi.Core.Ai;

namespace GemsAi.Core.Tasks
{
    public class StatusTask : ITask
    {
        private readonly IAiClient _client;

        public StatusTask(IAiClient client)
        {
            _client = client;
        }

        public bool CanHandle(string input)
        {
            var lowered = input.ToLower();
            return lowered.Contains("model") || lowered.Contains("who are you") || lowered.Contains("name");
        }

        public Task<string> ExecuteAsync(string input)
        {
            return Task.FromResult($"I'm currently powered by the `{_client.Model}` model via Ollama 🧠");
        }
    }
}