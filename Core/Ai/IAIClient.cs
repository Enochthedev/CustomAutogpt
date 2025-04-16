using System.Collections.Generic;
using System.Threading.Tasks;
using GemsAi.Core.TaskManagement.TaskCommands.Utils;

namespace GemsAi.Core.Ai
{
    public interface IAiClient
    {

        Task<string> GenerateAsync(string prompt);

        // New overload to allow model overrides
        // IAiClient.cs
        Task<Dictionary<string, string>> ExtractEntitiesAsync(string input);
        Task<Dictionary<string, string>> ExtractEntitiesAsync(string input, ErpModuleSchema schema);

        Task<List<string>> GetAllModelsAsync();
        string Model { get; }

        Task<string> DetectIntentAsync(string input);

    }
}