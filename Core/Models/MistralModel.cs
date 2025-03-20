using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoGPTDotNet.Core.AI.Models
{
    public class MistralModel : IAgentModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public MistralModel(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> GenerateResponse(string prompt)
        {
            var requestContent = new
            {
                prompt = prompt,
                max_tokens = 150
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestContent), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.PostAsync("https://api.mistral.ai/v1/completions", jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var jsonElement = JsonSerializer.Deserialize<JsonElement>(responseString);
            if (jsonElement.TryGetProperty("choices", out var choices) && 
                choices.GetArrayLength() > 0 && 
                choices[0].TryGetProperty("text", out var text))
            {
                return text.GetString() ?? string.Empty;
            }
            return string.Empty;
        }
    }
}