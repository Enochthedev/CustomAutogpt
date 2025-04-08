using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace GemsAI.Core.NLP.EntityExtraction
{
    public class EntityExtractor
    {
        private readonly List<string> commonWords = new List<string>();
        private readonly List<string> departmentPatterns = new List<string>();
        private readonly List<string> namePatterns = new List<string>();

        public EntityExtractor()
        {
            try
            {
                var config = LoadPatterns();
                commonWords = config?["commonWords"]?.ToObject<List<string>>() ?? new List<string>();
                departmentPatterns = config?["departmentPatterns"]?.ToObject<List<string>>() ?? new List<string>();
                namePatterns = config?["namePatterns"] != null
                    ? (config["namePatterns"]?.Select(p => p?["pattern"]?.ToString() ?? "").Where(p => !string.IsNullOrEmpty(p)).ToList() ?? new List<string>())
                    : new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patterns: {ex.Message}");
            }
        }

        private JObject LoadPatterns()
        {
            try
            {
                string filePath = Path.Combine(AppContext.BaseDirectory, "NLP/EntityExtraction/Patterns.json");
                if (File.Exists(filePath))
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JObject.Parse(jsonData);
                }
                else
                {
                    Console.WriteLine("Patterns.json not found.");
                    return new JObject();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading patterns file: {ex.Message}");
                return new JObject();
            }
        }

        public Dictionary<string, string> ExtractEntities(string input)
        {
            var entities = new Dictionary<string, string>();
            var cleanedInput = GemsAI.Core.NLP.Utils.TextCleaner.Clean(input);

            // Extract department using patterns
            foreach (var department in departmentPatterns)
            {
                if (!string.IsNullOrEmpty(department) && cleanedInput.Contains(department, StringComparison.OrdinalIgnoreCase))
                {
                    entities["Department"] = department;
                    break;
                }
            }

            // Extract names using dynamic patterns
            foreach (var pattern in namePatterns)
            {
                if (!string.IsNullOrEmpty(pattern))
                {
                    var matches = Regex.Matches(cleanedInput, pattern);
                    foreach (Match match in matches)
                    {
                        string candidate = match.Value;
                        if (!commonWords.Any(word => cleanedInput.StartsWith(word, StringComparison.OrdinalIgnoreCase)))
                        {
                            entities["Name"] = candidate;
                            return entities;
                        }
                    }
                }
            }

            return entities;
        }
    }
}