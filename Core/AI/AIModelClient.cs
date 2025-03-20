using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Diagnostics;

public class AIModelClient
{
    private readonly HttpClient _httpClient;
    private readonly string _aiApiUrl;
    private readonly ContextManager _contextManager;

    public AIModelClient(HttpClient httpClient, ContextManager contextManager, string aiApiUrl = "http://localhost:11434/api/generate") // ✅ Updated URL
    {
        _httpClient = httpClient;
        _contextManager = contextManager;
        _aiApiUrl = aiApiUrl;
    }

    public async Task<bool> IsRunningAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("http://localhost:11434/api/tags");
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking AI model status: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> TryStartModelAsync(string model)
    {
        try
        {
            // Check if the model is already running
            if (await IsRunningAsync())
            {
                Console.WriteLine($"AI model {model} is already running.");
                return true;
            }

            Console.WriteLine($"Attempting to start AI model: {model}");

            var process = new ProcessStartInfo
            {
                FileName = "/usr/bin/env",
                Arguments = $"ollama run {model}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process.Start(process);

            // Wait and verify the model starts correctly
            for (int i = 0; i < 5; i++)
            {
                await Task.Delay(3000);
                if (await IsRunningAsync())
                {
                    Console.WriteLine($"AI model {model} successfully started.");
                    return true;
                }
            }

            Console.WriteLine($"AI model {model} failed to start.");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting AI model: {ex.Message}");
            return false;
        }
    }

    public async Task<string> GenerateTextAsync(string prompt, string model = "tinyllama:latest") // ✅ Default model fixed
    {
        if (!await IsRunningAsync())
        {
            Console.WriteLine("AI model is not running. Trying to start it...");
            bool started = await TryStartModelAsync(model);
            if (!started)
            {
                return "Error: AI model is not running and could not be started.";
            }
        }

        _contextManager.AddToHistory(prompt); // ✅ Store prompt for context

        try
        {
            var requestBody = new
            {
                model = model,
                prompt = $"{_contextManager.GetContext()} {prompt}", // ✅ Include past context
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_aiApiUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseString))
            {
                return "Error: Empty response from AI model.";
            }

            try
            {
                var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseString);
                
                if (jsonResponse.TryGetProperty("message", out var messageProperty) &&
                    messageProperty.TryGetProperty("content", out var contentProperty) &&
                    contentProperty.ValueKind == JsonValueKind.String)
                {
                    return contentProperty.GetString() ?? "Error: AI response is null.";
                }

                return $"Error: Unexpected response format. Raw response: {responseString}";
            }
            catch (JsonException jsonEx)
            {
                return $"Error: Failed to parse AI response. Raw response: {responseString}. Exception: {jsonEx.Message}";
            }
        }
        catch (Exception ex)
        {
            return $"Error: Failed to communicate with AI model. {ex.Message}";
        }
    }
}