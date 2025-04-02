using GemsAi.Core.Tasks;

namespace GemsAi.Core.Ai
{
    public class LLMTask : ITask
    {
        private readonly IAiClient _ai;

        public LLMTask(IAiClient ai)
        {
            _ai = ai;
        }

        public bool CanHandle(string input) => true; // fallback task

        public async Task<string> ExecuteAsync(string input)
        {
            return await _ai.GenerateAsync(input);
        }
    }
}