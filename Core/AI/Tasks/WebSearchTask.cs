using System;
using System.Net.Http;
using System.Threading.Tasks;

public class WebSearchTask
{
    private readonly HttpClient _httpClient;

    public WebSearchTask(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task ExecuteAsync(string query)
    {
        await Task.Run(() =>
        {
            Console.WriteLine($"User requested a web search for: {query}");
            Console.WriteLine("Do you want to proceed? (yes/no)");

            string userResponse = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
            if (userResponse != "yes")
            {
                Console.WriteLine("Search cancelled.");
                return;
            }

            string searchUrl = $"https://www.google.com/search?q={Uri.EscapeDataString(query)}";
            Console.WriteLine($"Opening browser: {searchUrl}");
            
            // Opens the default web browser with the search URL
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = searchUrl,
                UseShellExecute = true
            });
        });
    }
}