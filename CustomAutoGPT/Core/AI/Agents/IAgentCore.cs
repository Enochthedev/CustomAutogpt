using AutoGPTDotNet.Core.Tasks;

namespace AutoGPTDotNet.Core.AI
{
    public interface IAgentCore
    {
        void SetGoal(string goal);
        void AddTask(ITask task);
        void ExecuteTasks();
    }
}