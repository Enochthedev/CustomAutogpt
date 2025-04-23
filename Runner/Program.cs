using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GemsAi.Core.Agent;
using GemsAi.Core.Memory;
using GemsAi.Core.Ai;
using GemsAi.Core.TaskManagement.TaskCommands;
using GemsAi.Core.LearnedTasks;
using GemsAi.Core.TaskManagement.TaskCommands.Erp;
using GemsAi.Core.Models;
using GemsAi.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

// ----- SETUP -----

// Read config
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddCommandLine(args)
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(configuration);
services.AddSingleton<HttpClient>();
services.AddSingleton<IMemoryStore, InMemoryMemoryStore>();

// AI client
var defaultModel = configuration["AI:DefaultModel"] ?? "smolLM2:1.7b";
services.AddSingleton<IAiClient>(sp => new OllamaClient(sp.GetRequiredService<HttpClient>(), defaultModel));

// Embedder setup (as before)
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

// Register ERP tasks (example: CreateEmployeeTask)
services.AddSingleton<IErpApiClient, DummyErpApiClient>();// Replace with your actual implementation
services.AddSingleton<ITask>(sp => new CreateEmployeeTask(
    sp.GetRequiredService<IAiClient>(),
    sp.GetRequiredService<IErpApiClient>(),
    sp.GetRequiredService<IConfiguration>()
));
//
services.AddSingleton<ITask>(sp => new UpdateEmployeeTask(
    sp.GetRequiredService<IAiClient>(),
    sp.GetRequiredService<IErpApiClient>(),
    sp.GetRequiredService<IConfiguration>()));

// Add other ITask implementations as needed

services.AddSingleton<IAgent>(sp =>
    new GemsAgent(
        sp.GetServices<ITask>().ToList(),
        sp.GetRequiredService<IMemoryStore>()
    )
);

var provider = services.BuildServiceProvider();
var tasks = provider.GetServices<ITask>().ToList();
// tasks.AddRange(LearnedTaskManager.LoadLearnedTasks());

var memory = provider.GetRequiredService<IMemoryStore>();
var agent = new GemsAgent(tasks, memory);

// ----- MODE SWITCH -----

// Determine run mode: Console or Web
bool runWeb = false;

// -- Check for command-line arg
if (args.Any(arg => arg.Contains("web", StringComparison.OrdinalIgnoreCase)))
    runWeb = true;

// -- OR config flag
if (!runWeb && configuration["AppMode"]?.Equals("Web", StringComparison.OrdinalIgnoreCase) == true)
    runWeb = true;

if (runWeb)
{
    // -------- WEB SERVER MODE --------
    var builder = WebApplication.CreateBuilder();
    
    foreach (var service in services)
        builder.Services.Add(service);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    app.UseSwagger();
    app.UseSwaggerUI();

    // /ai endpoint (ERP)
    app.MapPost("/ai", async (GemsAi.Core.Models.PromptRequest req, IAgent agent) =>
    {
        if (string.IsNullOrWhiteSpace(req.Input))
            return Results.BadRequest("Input required.");

        try
        {
            var output = await agent.RunAsync(req.Input);
            return Results.Ok(output);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    })
    .WithName("RunErpTask")
    .WithOpenApi();

    // /chat endpoint (General LLM chat)
    app.MapPost("/chat", async (GemsAi.Core.Models.PromptRequest req, IAiClient aiClient) =>
    {
        if (string.IsNullOrWhiteSpace(req.Input))
            return Results.BadRequest("Input required.");

        try
        {
            var output = await aiClient.GenerateAsync(req.Input);
            return Results.Ok(output);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message);
        }
    })
    .WithName("AskAI")
    .WithOpenApi();

    Console.WriteLine("🤖 Gems AI Agent HTTP server running on http://localhost:5000/swagger");
    app.Run("http://localhost:5000");
}
else
{
    // -------- CONSOLE MODE --------
    Console.WriteLine("🤖 Gems AI Agent Ready! (Console Mode)");

    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) continue;

        try
    {
        // Step 1: Use Ollama to detect the intent
        var aiClient = provider.GetRequiredService<IAiClient>();
        var intent = await aiClient.DetectIntentAsync(input);

        // Step 2: Try AI intent-based dispatch
        var task = tasks.FirstOrDefault(t => t.CanHandleIntent(intent));
        if (task == null)
        {
            // Fallback: Try keyword CanHandle (legacy/compat)
            task = tasks.FirstOrDefault(t => t.CanHandle(input));
        }

        if (task != null)
        {
            var output = await task.ExecuteAsync(input);
            Console.WriteLine(output);
        }
        else
        {
            Console.WriteLine("No task could handle the input (intent: " + intent + ").");
        }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }
}