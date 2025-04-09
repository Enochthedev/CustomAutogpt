using System.Text.Json;
using GemsAi.Core.Ai;
using GemsAi.Core.TaskManagement.TaskCommands.Utils;

namespace GemsAi.Core.TaskManagement.TaskCommands
{
    public class ErpTask : ITask
    {
        private readonly IAiClient _client;
        private readonly string _moduleName;

        public ErpTask(IAiClient client, string moduleName)
        {
            _client = client;
            _moduleName = moduleName.ToLower();
        }

        public bool CanHandle(string input) => true;

        public async Task<string> ExecuteAsync(string input)
        {
            var modulePath = Path.Combine("Core", "NLP", "EntityExtraction", "ErpModules", _moduleName + ".json");
            if (!File.Exists(modulePath))
                return $"❌ Schema not found for module: {_moduleName}";

            var schemaJson = await File.ReadAllTextAsync(modulePath);
            var schema = JsonSerializer.Deserialize<ErpModuleSchema>(schemaJson);
            if (schema == null)
                return "❌ Failed to load schema.";

            var extractionPrompt = BuildPromptFromSchema(input, schema);
            var response = await _client.GenerateAsync(extractionPrompt, "gemma:2b" );

            Dictionary<string, string> parsed;
            try
            {
                parsed = JsonSerializer.Deserialize<Dictionary<string, string>>(response) ?? new();
            }
            catch
            {
                return $"❌ Could not parse AI response into expected format. Raw response:\n{response}";
            }

            var missing = schema.RequiredFields.Where(field => !parsed.ContainsKey(field)).ToList();
            if (missing.Any())
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"⚠️ Missing fields: {string.Join(", ", missing)}");
                Console.ResetColor();

                var additions = PromptForMissingFields(missing);
                foreach (var pair in additions)
                    parsed[pair.Key] = pair.Value;
            }

            return $"✅ Extracted Data for {_moduleName}:\n{JsonSerializer.Serialize(parsed, new JsonSerializerOptions { WriteIndented = true })}";
        }

        private string BuildPromptFromSchema(string input, ErpModuleSchema schema)
        {
            string required = string.Join(", ", schema.RequiredFields);
            string jsonExample = JsonSerializer.Serialize(schema.ExampleFormat, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            return $"""
            You are an API-connected AI agent.

            Given this user input:
            "{input}"

            Extract only the required fields for the ERP module `{_moduleName}`: {required}

            Respond **only** with compact JSON in the format:
            {jsonExample}

            Do not include explanations, preambles, or assistant language.
            Empty values are allowed but the keys must all be present.

            Respond with JSON only.
            """;
        }

        private Dictionary<string, string> PromptForMissingFields(List<string> missingFields)
        {
            var inputs = new Dictionary<string, string>();
            foreach (var field in missingFields)
            {
                Console.Write($"📝 Please enter value for '{field}': ");
                var value = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    inputs[field] = value;
                }
            }
            return inputs;
        }
    }
}