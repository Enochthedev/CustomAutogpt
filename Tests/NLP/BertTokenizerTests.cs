using Core.InProg.NLP.POS;
using Xunit;

namespace GemsAI.Tests.INProg.NLP
{
    public class BertTokenizerTests
    {
        private readonly BertTokenizer _tokenizer;

        public BertTokenizerTests()
        {
            _tokenizer = new BertTokenizer("Assets/Models/POS/vocab.txt"); // Update if path differs
        }

        [Fact]
        public void BasicTokenize_ShouldSplitPunctuation()
        {
            var tokens = _tokenizer.BasicTokenize("Hello, world!");
            Assert.Contains("hello", tokens);
            Assert.Contains(",", tokens);
            Assert.Contains("world", tokens);
        }

        [Fact]
        public void WordPieceTokenize_ShouldHandleKnownSubwords()
        {
            var result = _tokenizer.WordPieceTokenize("playing");
            Assert.NotEmpty(result); // should return ["play", "##ing"] if in vocab
        }

        [Fact]
        public void TokenizeToIds_ShouldHandleUnknowns()
        {
            var ids = _tokenizer.TokenizeToIds("unicorn");
            Assert.All(ids, id => Assert.True(id >= 0));
        }
    }
}