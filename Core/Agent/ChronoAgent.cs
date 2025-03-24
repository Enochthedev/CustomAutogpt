using GemsAi.Core.Tasks;
using GemsAi.Core.Memory;

namespace GemsAi.Core.Agent
{
    public class ChronoAgent : IAgent
    {
        private readonly List<ITask> _tasks;
        private readonly IMemoryStore _memory;

        public ChronoAgent(List<ITask> tasks, IMemoryStore memory)
        {
            _tasks = tasks;
            _memory = memory;
        }

        public async Task<string> RunAsync(string input)
        {
            await _memory.StoreAsync("user_input", input);

            foreach (var task in _tasks)
            {
                if (task.CanHandle(input))
                {
                    var result = await task.ExecuteAsync(input);
                    await _memory.StoreAsync("agent_response", result);
                    return result;
                }
            }

            return "No task could handle the input.";
        }
    }
}
