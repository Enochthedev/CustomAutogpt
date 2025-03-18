using OpenAI;
using OpenAI.Chat;
using AutoGPTDotNet.Core.AI;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace AutoGPTDotNet.Core.AI
{
    public class OpenAIModel : IAgentModel
    {
        private readonly ChatClient _chatClient;

        public OpenAIModel(string apiKey)
        {
            // Initialize the OpenAI client
            OpenAIClient openAiClient = new(apiKey);
            // Retrieve the ChatClient for the desired model
            _chatClient = openAiClient.GetChatClient("gpt-4");
        }

        public async Task<string> GenerateResponse(string input)
        {
            // Create a list of chat messages with the user's input
            var messages = new List<ChatMessage>
            {
                new UserChatMessage(input)
            };

            // Request a chat completion
            ChatCompletion chatCompletion = await _chatClient.CompleteChatAsync(messages);

            // Extract and return the assistant's reply
            return chatCompletion.Content[0].Text;
        }
    }
}