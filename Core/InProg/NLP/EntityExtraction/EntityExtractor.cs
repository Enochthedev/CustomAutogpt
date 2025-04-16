using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Core.InProg.NLP.EntityExtraction
{
    public class EntityExtractor
    {
        private readonly Dictionary<string, string> _schema;

        public EntityExtractor(string schemaPath = "Assets/NLP/EntitySchema.json")
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, schemaPath);
            string json = File.ReadAllText(fullPath);
            _schema = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!;
        }

        // public Dictionary<string, string> ExtractEntities(string text)
        // {
        //     var result = new Dictionary<string, string>();

        //     foreach (var field in _schema)
        //     {
        //         string entity = field.Key;
        //         string pattern = field.Value;

        //         var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
        //         if (match.Success)
        //         {
        //             result[entity] = match.Groups[1].Value;
        //         }
        //     }

        //     return result;
        // }
            public List<(string Entity, string Type)> ExtractEntities(string text)
        {
            var result = new List<(string Entity, string Type)>();

            foreach (var field in _schema)
            {
                string entity = field.Key;
                string pattern = field.Value;

                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    result.Add((match.Groups[1].Value, entity));
                }
            }

            return result;
        }
    }
}