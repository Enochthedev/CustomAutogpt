namespace GemsAi.Core.Agent
{
    public interface IAgent
    {
        Task<string> RunAsync(string input);
    }
}
