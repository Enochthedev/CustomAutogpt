using GemsAi.Core.LearnedTasks;
using GemsAi.Core.Tasks;
using GemsAi.Core.TaskManagement.TaskCommands;

namespace GemsAi.Core.Tasks
{
    public class TaskMetadataQueryTask : ITask
    {
        public bool CanHandle(string input)
        {
            var lowered = input.ToLower();
            return lowered.Contains("what model did you use to create it") ||
                   lowered.Contains("what model created the task");
        }
        public bool CanHandleIntent(string intent)
        {
            return false; // Not handled by default
        }

        public Task<string> ExecuteAsync(string input)
        {
            var modelUsed = LearnedTaskMetadataManager.GetLastCreatedTaskModel();
            if (modelUsed != null)
                return Task.FromResult($"The last generated task was created using the `{modelUsed}` model.");
            return Task.FromResult("No metadata found for the last generated task.");
        }
    }
}