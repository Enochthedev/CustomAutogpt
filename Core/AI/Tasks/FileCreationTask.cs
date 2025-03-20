using System;
using System.IO;
using System.Threading.Tasks;

public class FileCreationTask
{
    private readonly AIModelClient _aiModelClient;

    public FileCreationTask(AIModelClient aiModelClient)
    {
        _aiModelClient = aiModelClient;
    }

    public async Task ExecuteAsync(string filePath, string topic)
    {
        string prompt = $"Write a detailed document about {topic}.";
        string aiGeneratedText = await _aiModelClient.GenerateTextAsync(prompt);

        Console.WriteLine($"AI generated content:\n{aiGeneratedText}");
        Console.WriteLine("Do you want to save this to a file? (yes/no)");

        string userResponse = (Console.ReadLine() ?? string.Empty).Trim().ToLower();
        if (userResponse == "yes")
        {
            await File.WriteAllTextAsync(filePath, aiGeneratedText);
            Console.WriteLine($"File saved: {filePath}");
        }
        else
        {
            Console.WriteLine("File creation cancelled.");
        }
    }
}