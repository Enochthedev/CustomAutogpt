using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using GemsAi.Core.TaskManagement.TaskCommands.Utils;
namespace GemsAi.Core.Ai
{
    public class OllamaClient : IAiClient
    {
        public async Task<string> GenerateAsync(string prompt)
        {
            return await GenerateAsync(prompt, null);
        }
    private readonly HttpClient _http;
        private readonly string _model;
        public string Model => _model;

        public OllamaClient(HttpClient httpClient, string? model = null)
        {
            _http = httpClient;
            if (!CheckOllamaRunningAsync().GetAwaiter().GetResult())
                    throw new Exception("🛑 Ollama server is not running. Please start it with `ollama serve`.");

            _model = model ?? GetAvailableModelsAsync().GetAwaiter().GetResult() 
                ?? throw new Exception("No running models found in Ollama.");
        }

        public async Task<string> GenerateAsync(string prompt, string? modelOverride = null)
        {
            var modelToUse = modelOverride ?? _model;
            
            var response = await _http.PostAsJsonAsync("http://localhost:11434/api/generate", new
            {
                model = modelToUse,
                prompt = prompt,
                stream = false
            });

            if (!response.IsSuccessStatusCode)
                throw new Exception("Ollama API failed: " + response.StatusCode);

            var json = await response.Content.ReadFromJsonAsync<OllamaResponse>();
            return json?.Response ?? "No response.";
        }
        public async Task<string> DetectIntentAsync(string input)
        {
            string prompt = $"Detect the intent of the following text: \"{input}\"";
            return await GenerateAsync(prompt, "smolLM2");
        }

        public async Task<List<string>> GetAllModelsAsync()
        {
            var response = await _http.GetAsync("http://localhost:11434/api/tags");
            if (!response.IsSuccessStatusCode) return new List<string>();

            var data = await response.Content.ReadFromJsonAsync<ModelsResponse>();
            return data?.Models?.Select(m => m.Name).ToList() ?? new List<string>();
        }

        public async Task<Dictionary<string, string>> ExtractEntitiesAsync(string input, ErpModuleSchema schema)
        {
            string required = string.Join(", ", schema.RequiredFields);
            string jsonExample = JsonSerializer.Serialize(schema.ExampleFormat);

            string prompt = $"""
            You are a smart ERP NLP engine. Given the following sentence:

            "{input}"

            Extract and return the following fields: {required}

            Respond only with JSON in this format:
            {jsonExample}
            """;

            string response = await GenerateAsync(prompt, "gemma:2b");

            try
            {
                using var doc = JsonDocument.Parse(response);
                var result = new Dictionary<string, string>();
                foreach (var prop in doc.RootElement.EnumerateObject())
                    result[prop.Name] = prop.Value.GetString() ?? "";
                return result;
            }
            catch
            {
                return new Dictionary<string, string> { { "intent", "unknown" } };
            }
        }
        public async Task<Dictionary<string, string>> ExtractEntitiesAsync(string input)
        {
            // Default fallback schema (can be improved)
            var defaultSchema = new ErpModuleSchema
            {
                RequiredFields = new List<string> { "intent", "name", "department" },
                ExampleFormat = new Dictionary<string, string>
                {
                    { "intent", "onboarding" },
                    { "name", "John Doe" },
                    { "department", "HR" }
                }
            };
            return await ExtractEntitiesAsync(input, defaultSchema);
        }

        private async Task<string?> GetAvailableModelsAsync()
        {
            var response = await _http.GetAsync("http://localhost:11434/api/tags");
            if (!response.IsSuccessStatusCode) return null;

            var data = await response.Content.ReadFromJsonAsync<ModelsResponse>();
            return data?.Models?.FirstOrDefault()?.Name;
        }

        private async Task<bool> CheckOllamaRunningAsync()
        {
            try
            {
                var response = await _http.GetAsync("http://localhost:11434/api/tags");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private class OllamaResponse
        {
            public string Response { get; set; } = "";
        }

        private class ModelsResponse
        {
            [JsonPropertyName("models")]
            public List<ModelTag> Models { get; set; } = new();
        }

        private class ModelTag
        {
            [JsonPropertyName("name")]
            public string Name { get; set; } = "";
        }
    }
}