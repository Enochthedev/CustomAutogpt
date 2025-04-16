using Core.InProg.NLP.POS;
using Core.InProg.NLP.Tokenization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using APItesting.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Tokenizer>();
builder.Services.AddSingleton<PosTagger>();

var app = builder.Build();

app.MapPost("/tokenize", (TextRequest request, Tokenizer tokenizer) =>
{
    Console.WriteLine($"Tokenizing text: {request.Text}");
    var tokens = tokenizer.Tokenize(request.Text);
    return Results.Ok(new { tokens });
});

app.MapPost("/pos", (TextRequest request, PosTagger tagger) =>
{
    var result = tagger.Predict(request.Text);
    return Results.Ok(result);
});

app.Run();