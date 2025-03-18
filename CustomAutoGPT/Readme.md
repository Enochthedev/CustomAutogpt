using OpenAI.Managers;
using OpenAI.ObjectModels;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Interfaces;
using AutoGPTDotNet.Core.AI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoGPTDotNet.Infrastructure.AI
{
    public class OpenAIModel : IAgentModel
    {
        private readonly IOpenAIService _client;

        public OpenAIModel(string apiKey)
        {
            _client = new OpenAIService(new OpenAiOptions { ApiKey = apiKey });
        }

        public async Task<string> GenerateResponse(string input)
        {
            var messages = new List<ChatMessage>
            {
                ChatMessage.FromUser(input)
            };

            var chatRequest = new ChatCompletionCreateRequest
            {
                Messages = messages,
                Model = Models.Gpt_4
            };

            var chatResponse = await _client.ChatCompletion.CreateCompletion(chatRequest);

            // Ensure the response is valid
            if (chatResponse == null || chatResponse.Choices == null || !chatResponse.Choices.Any())
            {
                throw new System.Exception("OpenAI API call failed or returned an empty response.");
            }

            // Extract and return the assistant's reply safely
            return chatResponse.Choices.FirstOrDefault()?.Message?.Content ?? "No response from OpenAI";
        }
    }
}
