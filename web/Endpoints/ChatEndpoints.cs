using GemsAi.Core.Models;
using GemsAi.Core.Agent;
using GemsAi.Core.Ai;
using Microsoft.AspNetCore.Http;
public static class ChatEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/chat", async (PromptRequest req, IAiClient aiClient) =>
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
        }).WithName("AskAI").WithOpenApi();
    }
}