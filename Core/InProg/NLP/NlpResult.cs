namespace Core.InProg.NLP
{
    public class NlpResult
    {
        public string Intent { get; set; } = string.Empty;

        public List<(string Entity, string Type)> Entities { get; set; } = new();

        public List<(string Word, string Tag)> PosTags { get; set; } = new();
    }
}