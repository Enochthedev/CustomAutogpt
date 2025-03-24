using System.Text.RegularExpressions;

namespace GemsAi.Core.AI
{
    public class ModelRouter
    {
        private readonly List<string> _availableModels;

        public ModelRouter(List<string> availableModels)
        {
            _availableModels = availableModels;
        }

        public string PickBestModel(string taskDescription)
        {
            foreach (var kv in ModelRegistry.KnownModels)
            {
                if (_availableModels.Contains(kv.Key) &&
                    Regex.IsMatch(taskDescription, kv.Value.Strengths, RegexOptions.IgnoreCase))
                {
                    return kv.Key;
                }
            }

            // fallback
            return _availableModels.FirstOrDefault() ?? "smolLM2";
        }
    }
}