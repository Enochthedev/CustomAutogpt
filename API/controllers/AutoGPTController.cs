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
    private readonly AIModelClient _aiModelClient;


    public AutoGPTController(IEnumerable<IAgentModel> aiModels, INLPProcessor nlpProcessor, AIModelClient aiModelClient)
    {
        _aiModels = new Dictionary<string, IAgentModel>();
        foreach (var model in aiModels)
        {
            _aiModels[model.GetType().Name] = model;
        }

        _nlpProcessor = nlpProcessor ?? throw new ArgumentNullException(nameof(nlpProcessor)); // ✅ Injected NLP processor
        _aiModelClient = aiModelClient ?? throw new ArgumentNullException(nameof(aiModelClient));
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateResponse([FromQuery] string model, [FromBody] string prompt)
    {
        Console.WriteLine($"Received request - Model: {model}, Prompt: {prompt}");

        if (string.IsNullOrEmpty(prompt))
        {
            return BadRequest("Prompt must be provided.");
        }

        string response;
        
        if (string.IsNullOrEmpty(model) || !_aiModels.TryGetValue(model, out var selectedModel))
        {
            Console.WriteLine("Invalid or missing AI model. Using default AIModelClient.");
            response = await _aiModelClient.GenerateTextAsync(prompt);
        }
        else
        {
            var nlpResult = await _nlpProcessor.ProcessPromptAsync(prompt);
            if (nlpResult.Intent == "file_creation")
            {
                response = "Would you like me to create a file with the extracted information?";
            }
            else if (nlpResult.Intent == "web_search")
            {
                response = "Do you want me to search the web for this topic?";
            }
            else
            {
                response = await _aiModelClient.GenerateTextAsync(prompt);
            }
        }

        Console.WriteLine($"Response: {response}");
        return Ok(new { response });
    }

    [HttpGet]
    public IActionResult TestEndpoint()
    {
        return Ok("AutoGPT API is working!");
    }
}