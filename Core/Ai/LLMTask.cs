using GemsAi.Core.Tasks;

namespace GemsAi.Core.AI
{
    public class LLMTask : ITask
    {
        private readonly IAIClient _ai;

        public LLMTask(IAIClient ai)
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