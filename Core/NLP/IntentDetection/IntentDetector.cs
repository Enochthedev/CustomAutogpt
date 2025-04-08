using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace GemsAI.Core.NLP.IntentDetection
{
    public class IntentDetector
    {
        private readonly Dictionary<string, List<string>> intentPatterns;

        public IntentDetector()
        {
            intentPatterns = LoadIntents();
        }

        private Dictionary<string, List<string>> LoadIntents()
        {
            string baseDirectory = AppContext.BaseDirectory;
            string[] potentialPaths = new[]
            {
                Path.Combine(baseDirectory, "NLP/IntentDetection/IntentPatterns.json"),
                Path.Combine(baseDirectory, "../../../NLP/IntentDetection/IntentPatterns.json"),
                Path.Combine(baseDirectory, "../../../../../NLP/IntentDetection/IntentPatterns.json"),
                Path.Combine(Environment.CurrentDirectory, "NLP/IntentDetection/IntentPatterns.json")
            };

            foreach (var path in potentialPaths)
            {
                if (File.Exists(path))
                {
                    string jsonData = File.ReadAllText(path);
                    var parsedObject = JObject.Parse(jsonData)?.ToObject<Dictionary<string, List<string>>>();
                    if (parsedObject == null)
                    {
                        throw new InvalidOperationException("Failed to parse intent patterns from JSON data.");
                    }
                    return parsedObject;
                }
            }

            throw new FileNotFoundException("IntentPatterns.json not found in expected paths.");
        }

        public string DetectIntent(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "unknown";

            input = input.ToLower();  // Normalize input

            foreach (var intent in intentPatterns)
            {
                foreach (var pattern in intent.Value)
                {
                    if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                    {
                        return intent.Key.ToLower();  // Return normalized intent
                    }
                }
            }
            return "unknown";  // Return normalized "unknown"
        }
    }
}