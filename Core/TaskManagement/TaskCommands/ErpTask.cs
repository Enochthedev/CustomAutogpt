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

            Dictionary<string, string> parsed;
            try
            {
                parsed = await _client.ExtractEntitiesAsync(input, schema);
            }
            catch (Exception ex)
            {
                return $"❌ Failed to extract entities: {ex.Message}";
            }

            var missing = schema.RequiredFields.Where(field => !parsed.ContainsKey(field) || string.IsNullOrWhiteSpace(parsed[field])).ToList();
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