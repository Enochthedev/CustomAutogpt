namespace GemsAi.Core.TaskManagement.TaskCommands
{
    public interface ITask
    {
        bool CanHandle(string input);
        Task<string> ExecuteAsync(string input);
    }
}
