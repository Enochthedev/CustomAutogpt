using System.Text;
using System.Text.RegularExpressions;

namespace Core.InProg.NLP.POS
{
    public class BertTokenizer
    {
        private readonly Dictionary<string, int> _vocab;
        private readonly string _unkToken = "[UNK]";

        public BertTokenizer(string vocabPath)
        {
            _vocab = File.ReadAllLines(vocabPath)
                         .Select((token, idx) => new { token, idx })
                         .ToDictionary(x => x.token, x => x.idx);
        }

        public List<string> BasicTokenize(string text)
        {
            text = text.ToLower().Replace("’", "'").Replace("“", "\"").Replace("”", "\"");
            var spaced = Regex.Replace(text, @"([.,!?;:()\""])"," $1 ");
            var tokens = spaced.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            return tokens.ToList();
        }

        public List<string> WordPieceTokenize(string word)
        {
            if (_vocab.ContainsKey(word))
                return new List<string> { word };

            var tokens = new List<string>();
            int start = 0;

            while (start < word.Length)
            {
                int end = word.Length;
                string? currSubstring = null;

                while (start < end)
                {
                    var substr = word.Substring(start, end - start);
                    if (start > 0)
                        substr = "##" + substr;

                    if (_vocab.ContainsKey(substr))
                    {
                        currSubstring = substr;
                        break;
                    }

                    end--;
                }

                if (currSubstring == null)
                    return new List<string> { _unkToken };

                tokens.Add(currSubstring);
                start = end;
            }

            return tokens;
        }

        public List<int> TokenizeToIds(string text)
        {
            var words = BasicTokenize(text);
            var wordpieceTokens = new List<string>();

            foreach (var word in words)
            {
                wordpieceTokens.AddRange(WordPieceTokenize(word));
            }

            return wordpieceTokens.Select(t => _vocab.ContainsKey(t) ? _vocab[t] : _vocab[_unkToken]).ToList();
        }
    }
}