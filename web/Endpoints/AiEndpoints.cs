using GemsAi.Core.Models;
using GemsAi.Core.Agent;
using GemsAi.Core.Ai;
using Microsoft.AspNetCore.Http;
public static class AiEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapPost("/ai", async (PromptRequest req, IAgent agent) =>
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
        }).WithName("RunErpTask").WithOpenApi();

        // Add stream endpoint here if needed
    }
}