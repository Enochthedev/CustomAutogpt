namespace AutoGPTDotNet.Core.NLP
{
    public interface INLPProcessor
    {
        Task<NLPResult> ProcessPromptAsync(string prompt);
    }
}