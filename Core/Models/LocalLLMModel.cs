using System.Threading.Tasks;

namespace AutoGPTDotNet.Core.AI.Models
{
    public class LocalLLMModel : IAgentModel
    {
        public async Task<string> GenerateResponse(string prompt)
        {
            // Simulated Local AI Model Response (Replace with real logic)
            await Task.Delay(100); // Simulate processing time
            return $"[Local AI Response]: {prompt}";
        }
    }
}