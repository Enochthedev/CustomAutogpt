namespace GemsAi.Core.TaskManagement.TaskCommands
{
    public interface ITask
    {
        bool CanHandle(string input);
        bool CanHandleIntent(string intent);

        Task<string> ExecuteAsync(string input);
    }
}
