namespace GemsAi.Core.AI
{
    public interface IAIClient
    {
        Task<string> GenerateAsync(string prompt);
    }
}