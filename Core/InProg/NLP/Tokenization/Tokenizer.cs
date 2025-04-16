namespace Core.InProg.NLP.Tokenization
{
    public class Tokenizer
    {
        public List<string> Tokenize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var tokens = text
                .ToLower()
                .Split(new[] { ' ', '\t', '\r', '\n', '.', ',', '!', '?', ';', ':', '-', '(', ')', '[', ']', '{', '}', '"' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            return tokens;
        }
    }
}