using Xunit;
using Core.InProg.NLP.POS;

namespace GemsAI.Tests.INProg.NLP
{
    public class PosTaggerTests
    {
        private readonly PosTagger _tagger;

        public PosTaggerTests()
        {
            _tagger = new PosTagger(
                "Assets/Models/POS/bert-pos.onnx",
                "Assets/Models/POS/vocab.txt"
            );
        }

        [Fact]
        public void Predict_ValidSentence_ReturnsTaggedTokens()
        {
            var sentence = "The quick brown fox";
            var result = _tagger.Predict(sentence);

            Assert.NotEmpty(result);
            Assert.Equal(4, result.Count); // 4 words expected
            foreach (var (word, tag) in result)
            {
                Assert.False(string.IsNullOrWhiteSpace(word));
                Assert.False(string.IsNullOrWhiteSpace(tag));
            }
        }

        [Fact]
        public void Predict_EmptySentence_ReturnsEmptyList()
        {
            var result = _tagger.Predict("");
            Assert.Empty(result);
        }
    }
}