using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GemsAi.Core.Agent;
using GemsAi.Core.Memory;
using GemsAi.Core.AI;
using GemsAi.Core.Tasks;
using GemsAi.Core.LearnedTasks;
// assuming your embedders are in this namespace

// Build configuration from appsettings.json
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();

// Register configuration so it can be injected later if needed
services.AddSingleton<IConfiguration>(configuration);

// Register shared services
services.AddSingleton<HttpClient>();
services.AddSingleton<IMemoryStore, InMemoryMemoryStore>();

// Register the AI client using Ollama
// Optionally, you might read the default model from configuration:
var defaultModel = configuration["AI:DefaultModel"] ?? "smollm2:1.7b";
services.AddSingleton<IAIClient>(sp => new OllamaClient(
    sp.GetRequiredService<HttpClient>(), defaultModel));

// Register the embedder based on configuration
bool useOllamaEmbedder = bool.TryParse(configuration["Embedding:UseOllama"], out var useOllama) && useOllama;
if (useOllamaEmbedder)
{
    // Use the Ollama embedder and get endpoint from config
    // Example: Reading endpoint from configuration or using a default.
    string ollamaEndpoint = configuration["Embedding:OllamaEndpoint"] ?? "http://localhost:11434";
    services.AddSingleton<IEmbedder>(sp => new OllamaEmbedder(
        sp.GetRequiredService<HttpClient>(), ollamaEndpoint));
}
else
{
    // Use ML.NET embedder with the provided ONNX model path
    string onnxPath = configuration["Embedding:ONNXModelPath"] ?? "path/to/your/model.onnx";
    services.AddSingleton<IEmbedder>(sp => new MLNetEmbedder(onnxPath));
}

// Register the vector memory store
services.AddSingleton<IVectorMemoryStore, InMemoryVectorMemoryStore>();

// Use Scrutor to scan for all ITask implementations in the assembly
services.Scan(scan => scan
    .FromAssemblyOf<ITask>() // scans the assembly where ITask is defined
    .AddClasses(classes => classes.AssignableTo<ITask>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime()
);

// Register the agent
services.AddSingleton<IAgent, GemsAgent>();

// Build the service provider
var serviceProvider = services.BuildServiceProvider();

// Get all tasks (including dynamically registered ones)
var tasks = serviceProvider.GetServices<ITask>().ToList();

// Additionally, load learned tasks and add them if you want:
tasks.AddRange(LearnedTaskManager.LoadLearnedTasks());

// Manually create the agent with the DI tasks and memory store
var memory = serviceProvider.GetRequiredService<IMemoryStore>();
var agent = new GemsAgent(tasks, memory);

Console.WriteLine("🤖 Gems AI Agent Ready!");

// Main loop
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input))
        continue;

    try
    {
        var output = await agent.RunAsync(input);
        Console.WriteLine(output);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
        Console.ResetColor();
    }
}