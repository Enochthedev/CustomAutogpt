using System.Text.Json;
using GemsAi.Core.Ai;
using GemsAi.Core.NLP.EntityExtraction;
using Microsoft.Extensions.Configuration;

namespace GemsAi.Core.TaskManagement.TaskCommands.Erp
{
    public abstract class ErpTaskBase : ITask
    {
        protected readonly IAiClient _client;
        protected readonly string _moduleName;
        protected readonly IConfiguration _configuration;

        protected ErpTaskBase(IAiClient client, string moduleName, IConfiguration configuration)
        {
            _client = client;
            _moduleName = moduleName.ToLower();
            _configuration = configuration;
        }

        public abstract bool CanHandleIntent(string intent);
        public abstract bool CanHandle(string input);

        public async Task<string> ExecuteAsync(string input)
        {
            // Use config-based path!
            var schemaDirectory = _configuration["NLP:SchemaDirectory"];
            var modulePath = Path.Combine(schemaDirectory, _moduleName + ".json");
            Console.WriteLine($"[DEBUG] Current directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"[DEBUG] Looking for schema at: {Path.GetFullPath(modulePath)}");

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

            var missing = schema.RequiredFields
                .Where(field => !parsed.ContainsKey(field) || string.IsNullOrWhiteSpace(parsed[field]))
                .ToList();

            if (missing.Any())
            {
                var additions = PromptForMissingFields(missing);
                foreach (var pair in additions)
                    parsed[pair.Key] = pair.Value;
            }

            // Call the concrete ERP action (implemented by derived class)
            return await HandleErpOperationAsync(parsed);
        }

        protected abstract Task<string> HandleErpOperationAsync(Dictionary<string, string> parsed);

        protected virtual Dictionary<string, string> PromptForMissingFields(List<string> missingFields)
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
        protected async Task EnsureRequiredFieldsFilled(Dictionary<string, string> parsed, List<string> requiredFields, bool isConsole)
        {
            foreach (var field in requiredFields)
            {
                while (!parsed.ContainsKey(field) || string.IsNullOrWhiteSpace(parsed[field]))
                {
                    if (isConsole)
                    {
                        Console.Write($"📝 Please enter value for '{field}': ");
                        var value = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(value))
                            parsed[field] = value;
                        else
                            Console.WriteLine($"'{field}' is required.");
                    }
                    else
                    {
                        throw new Exception($"Missing required field: {field}");
                    }
                }
            }
        }
    }
}