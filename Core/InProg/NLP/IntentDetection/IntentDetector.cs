using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Core.InProg.NLP.IntentDetection
{
    public class IntentDetector
    {
        private readonly Dictionary<string, List<string>> _patterns;

        public IntentDetector(string patternPath = "Assets/NLP/IntentPatterns.json")
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, patternPath);
            string json = File.ReadAllText(fullPath);
            _patterns = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json)!;
        }

        public string DetectIntent(string sentence)
        {
            sentence = sentence.ToLower();
            foreach (var pattern in _patterns)
            {
                if (pattern.Value.Any(p => sentence.Contains(p.ToLower())))
                {
                    return pattern.Key;
                }
            }
            return "unknown";
        }
    }
}