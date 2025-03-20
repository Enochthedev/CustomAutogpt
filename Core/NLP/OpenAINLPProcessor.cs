using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AutoGPTDotNet.Core.NLP{
    public class OpenAINLPProcessor : INLPProcessor
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;
        private const string Model = "gpt-4o"; // You can change this

        public OpenAINLPProcessor(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _openAiApiKey = apiKey;
        }

        public async Task<NLPResult> ProcessPromptAsync(string prompt)
        {
            var requestBody = new
            {
                model = Model,
                messages = new[]
                {
                    new { role = "system", content = "You are an NLP module that extracts intent and key entities from user prompts." },
                    new { role = "user", content = $"Analyze this prompt: {prompt}" }
                },
                temperature = 0.2
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _openAiApiKey);
            
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            var responseString = await response.Content.ReadAsStringAsync();

            // Process response (assuming JSON format)
            var responseObject = JsonSerializer.Deserialize<JsonElement>(responseString);
            string nlpAnalysis = responseObject.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString() ?? string.Empty;

            // Extract structured data from OpenAI response
            var nlpResult = new NLPResult
            {
                Intent = ExtractIntent(nlpAnalysis),
                Entities = ExtractEntities(nlpAnalysis),
                ProcessedPrompt = prompt 
            };

            return nlpResult;
        }

        private string ExtractIntent(string response)
        {
            if (response.Contains("summarize", StringComparison.OrdinalIgnoreCase)) return "summarization";
            if (response.Contains("generate", StringComparison.OrdinalIgnoreCase)) return "text_generation";
            if (response.Contains("fetch", StringComparison.OrdinalIgnoreCase)) return "data_fetching";
            return "general";
        }

        private Dictionary<string, string> ExtractEntities(string response)
        {
            var entities = new Dictionary<string, string>();

            if (response.Contains("date:", StringComparison.OrdinalIgnoreCase))
            {
                string date = response.Split("date:")[1].Split("\n")[0].Trim();
                entities["date"] = date;
            }

            if (response.Contains("topic:", StringComparison.OrdinalIgnoreCase))
            {
                string topic = response.Split("topic:")[1].Split("\n")[0].Trim();
                entities["topic"] = topic;
            }

            return entities;
        }
    }
}