using Microsoft.AspNetCore.Mvc;
using AutoGPTDotNet.Core.AI;
using System.Collections.Generic;

[ApiController]
[Route("api/autogpt")]
public class AutoGPTController : ControllerBase
{
    private readonly Dictionary<string, IAgentModel> _aiModels;

    public AutoGPTController(IEnumerable<IAgentModel> aiModels)
    {
        _aiModels = new Dictionary<string, IAgentModel>();
        foreach (var model in aiModels)
        {
            _aiModels[model.GetType().Name] = model;
        }
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateResponse([FromQuery] string model, [FromBody] string prompt)
    {
        if (!_aiModels.TryGetValue(model, out var selectedModel))
        {
            return BadRequest("Invalid AI model specified.");
        }

        var response = await selectedModel.GenerateResponse(prompt);
        return Ok(new { response });
    }
}