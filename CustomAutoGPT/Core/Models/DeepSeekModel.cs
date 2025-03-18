using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoGPTDotNet.Core.AI.Models
{
    public class DeepSeekModel : IAgentModel
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public DeepSeekModel(HttpClient httpClient, string apiKey)
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

            var response = await _httpClient.PostAsync("https://api.deepseek.com/v1/completions", jsonContent);
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<JsonElement>(responseString).GetProperty("text").GetString();
        }
    }
}