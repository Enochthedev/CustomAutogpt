using AutoGPTDotNet.Core.AI.Models;
using AutoGPTDotNet.Core.AI;
using AutoGPTDotNet.Core.NLP;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers(); // Register Controllers

// Register AI Models
builder.Services.AddSingleton<HttpClient>();
// builder.Services.AddSingleton<IAgentModel, OpenAIModel>();  // Default AI Model
// builder.Services.AddSingleton<IAgentModel, DeepSeekModel>();
// builder.Services.AddSingleton<IAgentModel, MistralModel>();
// builder.Services.AddSingleton<IAgentModel, ClaudeModel>();
// builder.Services.AddSingleton<IAgentModel, LocalLLMModel>();

builder.Services.AddSingleton<IAgentModel>(sp =>
    new OpenAIModel(sp.GetRequiredService<IConfiguration>().GetValue<string>("AIModels:OpenAI:ApiKey") ?? throw new ArgumentNullException("AIModels:OpenAI:ApiKey"))
);
builder.Services.AddSingleton<IAgentModel>(sp =>
    new DeepSeekModel(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IConfiguration>().GetValue<string>("AIModels:DeepSeek:ApiKey") ?? throw new ArgumentNullException("AIModels:DeepSeek:ApiKey"))
);
builder.Services.AddSingleton<IAgentModel>(sp =>
    new MistralModel(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IConfiguration>().GetValue<string>("AIModels:Mistral:ApiKey") ?? throw new ArgumentNullException("AIModels:Mistral:ApiKey"))
);
builder.Services.AddSingleton<IAgentModel>(sp =>
    new ClaudeModel(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IConfiguration>().GetValue<string>("AIModels:Claude:ApiKey") ?? throw new ArgumentNullException("AIModels:Claude:ApiKey"))
);
builder.Services.AddSingleton<INLPProcessor>(sp =>
    new OpenAINLPProcessor(
        sp.GetRequiredService<HttpClient>(),
        sp.GetRequiredService<IConfiguration>().GetValue<string>("AIModels:OpenAI:ApiKey") ?? throw new ArgumentNullException("OpenAI API Key is missing.")
    )
);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();