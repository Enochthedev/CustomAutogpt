using System.Collections.Generic;

namespace GemsAi.Core.NLP.EntityExtraction
{
    public class ErpModuleSchema
    {
        public List<string> RequiredFields { get; set; } = new();
        public Dictionary<string, string> ExampleFormat { get; set; } = new();
    }
}