using Microsoft.AspNetCore.Mvc;
using AutoGPTDotNet.Core.AI;
using AutoGPTDotNet.Core.NLP;
using System.Collections.Generic;

[ApiController]
[Route("api/autogpt")]
public class AutoGPTController : ControllerBase
{
    private readonly Dictionary<string, IAgentModel> _aiModels;
    private readonly INLPProcessor _nlpProcessor; // ✅ Added missing field

    public AutoGPTController(IEnumerable<IAgentModel> aiModels, INLPProcessor nlpProcessor)
    {
        _aiModels = new Dictionary<string, IAgentModel>();
        foreach (var model in aiModels)
        {
            _aiModels[model.GetType().Name] = model;
        }

        _nlpProcessor = nlpProcessor ?? throw new ArgumentNullException(nameof(nlpProcessor)); // ✅ Injected NLP processor
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateResponse([FromQuery] string model, [FromBody] string prompt)
    {
        Console.WriteLine($"Received request - Model: {model}, Prompt: {prompt}");

        if (string.IsNullOrEmpty(model) || string.IsNullOrEmpty(prompt))
        {
            Console.WriteLine("Invalid request - Missing model or prompt.");
            return BadRequest("Model and prompt must be provided.");
        }

        if (!_aiModels.TryGetValue(model, out var selectedModel))
        {
            Console.WriteLine("Invalid AI model specified.");
            return BadRequest("Invalid AI model specified.");
        }

        // ✅ Now `_nlpProcessor` is available
        var nlpResult = await _nlpProcessor.ProcessPromptAsync(prompt);
        Console.WriteLine($"NLP Result - Intent: {nlpResult.Intent}, Entities: {string.Join(", ", nlpResult.Entities)}");

        try
        {
            var response = await selectedModel.GenerateResponse(nlpResult.ProcessedPrompt);
            Console.WriteLine($"Response: {response}");
            return Ok(new { response, nlpResult });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet]
    public IActionResult TestEndpoint()
    {
        return Ok("AutoGPT API is working!");
    }
}