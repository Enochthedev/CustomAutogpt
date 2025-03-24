using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemsAi.Core.AI
{
    public interface IAIClient
    {

        Task<string> GenerateAsync(string prompt);

        // New overload to allow model overrides
        Task<string> GenerateAsync(string prompt, string? modelOverride);

        // New method to fetch all available models
        Task<List<string>> GetAllModelsAsync();
        string Model { get; }
    }
}