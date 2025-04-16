namespace Core.InProg.NLP
{
    public class NERProcessor
    {
        public Dictionary<string, string> Run(string input)
        {
            return new Dictionary<string, string>
            {
                { "Elon Musk", "Person" },
                { "Tesla", "Organization" },
                { "California", "Location" }
            };
        }
    }
}