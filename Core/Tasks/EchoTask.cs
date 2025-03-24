namespace GemsAi.Core.Tasks
{
    public class EchoTask : ITask
    {
        public bool CanHandle(string input) => input.StartsWith("echo");

        public Task<string> ExecuteAsync(string input)
        {
            return Task.FromResult("Echo: " + input);
        }
    }
}
