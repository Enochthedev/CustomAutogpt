namespace GemsAi.Core.Tasks
{
    public interface ITask
    {
        bool CanHandle(string input);
        Task<string> ExecuteAsync(string input);
    }
}
