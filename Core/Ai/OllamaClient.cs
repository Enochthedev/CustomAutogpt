using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Linq;
using System.Threading.Tasks;
using GemsAi.Core.NLP.EntityExtraction;

namespace GemsAi.Core.Ai
{
    public class OllamaClient : IAiClient
    {
        private readonly HttpClient _http;
        private readonly string _model;
        public string Model => _model;

        public OllamaClient(HttpClient httpClient, string? model = null)
        {
            _http = httpClient;
            if (!CheckOllamaRunningAsync().GetAwaiter().GetResult())
                throw new Exception("🛑 Ollama server is not running. Please start it with `ollama serve`.");

            // Use available models, or provided one if valid
            var availableModel = GetAvailableModelsAsync().GetAwaiter().GetResult();
            _model = model ?? availableModel
                ?? throw new Exception("No running models found in Ollama.");
        }

        public async Task<string> GenerateAsync(string prompt)
            => await GenerateAsync(prompt, null);

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
            return json?.Response ?? "No Ai.";
        }

        public async Task<string> DetectIntentAsync(string input)
        {
            // Use the exact name you have installed for intent detection!
            // If smollm2:1.7b is best for this, use that.
            string prompt = $"Given the following user request, return the most likely intent as a single lowercase snake_case string (e.g., add_employee, update_employee, delete_employee, etc):\n\"{input}\"";
            return await GenerateAsync(prompt, "smollm2:1.7b");
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
            var exampleFormat = schema.ExampleFormat ?? schema.RequiredFields.ToDictionary(f => f, f => $"<{f}>");

            string required = string.Join(", ", schema.RequiredFields);
            string jsonExample = JsonSerializer.Serialize(exampleFormat);

            string prompt = $"""
            You are a smart ERP data extractor.

            For this request:
            "{input}"

            Extract the following fields: {required}

            Respond ONLY with JSON in this format:
            {jsonExample}

            Fill as many fields as you can from the sentence. For missing fields, use an empty string.
            Never use generic values. Only extract exactly what is in the sentence. If unsure, leave blank.
            """;

            // Use a model with strong extraction capability, e.g. gemma:2b
            string aiResult = await GenerateAsync(prompt, "gemma:2b");
            try
            {
                using var doc = JsonDocument.Parse(aiResult);
                var result = new Dictionary<string, string>();
                foreach (var prop in doc.RootElement.EnumerateObject())
                    result[prop.Name] = prop.Value.GetString() ?? "";
                Console.WriteLine("[DEBUG] AI extracted: " + JsonSerializer.Serialize(result));
                return result;
            }
            catch (Exception)
            {
                return schema.RequiredFields.ToDictionary(f => f, f => "");
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