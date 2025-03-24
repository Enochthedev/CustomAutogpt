using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GemsAi.Core.Memory
{
    public class OllamaEmbedder : IEmbedder
    {
        private readonly HttpClient _http;
        private readonly string _endpoint;

        // Updated constructor: takes both HttpClient and endpoint URL.
        public OllamaEmbedder(HttpClient httpClient, string endpoint)
        {
            _http = httpClient;
            _endpoint = endpoint;
        }

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            // Use the endpoint provided in the constructor.
            var response = await _http.PostAsJsonAsync($"{_endpoint}/api/embed", new { text });
            if (!response.IsSuccessStatusCode)
                throw new Exception("Ollama embedder API failed: " + response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<EmbedResponse>();
            return result?.Embedding ?? new float[0];
        }

        private class EmbedResponse
        {
            public float[] Embedding { get; set; } = new float[0];
        }
    }
}