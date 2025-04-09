using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GemsAi.Core.Agent;
using GemsAi.Core.Memory;
using GemsAi.Core.Ai;
using GemsAi.Core.Tasks;
using GemsAi.Core.TaskManagement.TaskCommands;
using GemsAi.Core.LearnedTasks;
using GemsAi.Core.TaskManagement.TaskCommands.Utils; // For ErpModuleSchema if needed


// Build configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<HttpClient>();
services.AddSingleton<IMemoryStore, InMemoryMemoryStore>();

// Register AI client
var defaultModel = configuration["AI:DefaultModel"] ?? "smolLM2:1.7b";
services.AddSingleton<IAiClient>(sp => new OllamaClient(sp.GetRequiredService<HttpClient>(), defaultModel));

// Embedder
bool useOllamaEmbedder = bool.TryParse(configuration["Embedding:UseOllama"], out var useOllama) && useOllama;
if (useOllamaEmbedder)
{
    string endpoint = configuration["Embedding:OllamaEndpoint"] ?? "http://localhost:11434";
    services.AddSingleton<IEmbedder>(sp => new OllamaEmbedder(sp.GetRequiredService<HttpClient>(), endpoint));
}
else
{
    string modelPath = configuration["Embedding:ONNXModelPath"] ?? "path/to/model.onnx";
    services.AddSingleton<IEmbedder>(sp => new MLNetEmbedder(modelPath));
}
services.AddSingleton<IVectorMemoryStore, InMemoryVectorMemoryStore>();

// Register ITask implementations EXCEPT ErpTask (handled manually)
services.Scan(scan => scan
    .FromAssemblyOf<ITask>()
    .AddClasses(c => c.AssignableTo<ITask>().Where(t => t != typeof(ErpTask)))
    .AsImplementedInterfaces()
    .WithSingletonLifetime());

// Dynamically register ERP module tasks
var erpModulesDir = Path.Combine("Core", "NLP", "EntityExtraction", "ErpModules");
if (Directory.Exists(erpModulesDir))
{
    foreach (var file in Directory.GetFiles(erpModulesDir, "*.json"))
    {
        var moduleName = Path.GetFileNameWithoutExtension(file);
        services.AddSingleton<ITask>(sp => new ErpTask(sp.GetRequiredService<IAiClient>(), moduleName));
    }
}
else
{
    Console.WriteLine($"⚠️ ERP module folder not found: {erpModulesDir}");
}

services.AddSingleton<IAgent, GemsAgent>();

var provider = services.BuildServiceProvider();
var tasks = provider.GetServices<ITask>().ToList();
tasks.AddRange(LearnedTaskManager.LoadLearnedTasks());

var memory = provider.GetRequiredService<IMemoryStore>();
var agent = new GemsAgent(tasks, memory);

Console.WriteLine("🤖 Gems AI Agent Ready!");

// Main loop
while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(input)) continue;

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