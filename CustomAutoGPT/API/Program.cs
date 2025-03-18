using AutoGPTDotNet.Core.AI.Models;
using AutoGPTDotNet.Core.AI;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Register AI Models
builder.Services.AddSingleton<HttpClient>();
// builder.Services.AddSingleton<IAgentModel, OpenAIModel>();  // Default AI Model
// builder.Services.AddSingleton<IAgentModel, DeepSeekModel>();
// builder.Services.AddSingleton<IAgentModel, MistralModel>();
// builder.Services.AddSingleton<IAgentModel, ClaudeModel>();
// builder.Services.AddSingleton<IAgentModel, LocalLLMModel>();

builder.Services.AddSingleton<IAgentModel>(sp =>
    new OpenAIModel(sp.GetRequiredService<IConfiguration>().GetValue<string>("OpenAI:ApiKey") ?? throw new ArgumentNullException("OpenAI:ApiKey"))
);
builder.Services.AddSingleton<IAgentModel>(sp =>
    new DeepSeekModel(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IConfiguration>().GetValue<string>("DeepSeek:ApiKey") ?? throw new ArgumentNullException("DeepSeek:ApiKey"))
);
builder.Services.AddSingleton<IAgentModel>(sp =>
    new MistralModel(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IConfiguration>().GetValue<string>("Mistral:ApiKey") ?? throw new ArgumentNullException("Mistral:ApiKey"))
);
builder.Services.AddSingleton<IAgentModel>(sp =>
    new ClaudeModel(sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IConfiguration>().GetValue<string>("Claude:ApiKey") ?? throw new ArgumentNullException("Claude:ApiKey"))
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();