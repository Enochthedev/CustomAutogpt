namespace AutoGPTDotNet.Core.AI
{
    public interface IAgentModel
    {
        Task<string> GenerateResponse(string input);
    }
}
