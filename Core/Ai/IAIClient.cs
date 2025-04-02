using System.Collections.Generic;
using System.Threading.Tasks;

namespace GemsAi.Core.Ai
{
    public interface IAiClient
    {

        Task<string> GenerateAsync(string prompt);

        // New overload to allow model overrides
        Task<string> GenerateAsync(string prompt, string? modelOverride);

        Task<List<string>> GetAllModelsAsync();
        string Model { get; }

        Task<string> DetectIntentAsync(string input);
        Task<Dictionary<string, string>> ExtractEntitiesAsync(string input);
    }
}