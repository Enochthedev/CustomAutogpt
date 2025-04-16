using Xunit;
using System.Collections.Generic;
using Core.InProg.NLP.Tokenization;

namespace GemsAI.Tests.INProg.NLP
{
    public class TokenizerTests
    {
        private readonly Tokenizer tokenizer;

        public TokenizerTests()
        {
            tokenizer = new Tokenizer();
        }

        [Fact]
        public void Tokenize_BasicSentence_ReturnsCorrectTokens()
        {
            string input = "The quick brown fox jumps over the lazy dog.";
            var expected = new List<string> { "the", "quick", "brown", "fox", "jumps", "over", "the", "lazy", "dog" };

            var tokens = tokenizer.Tokenize(input);

            Assert.Equal(expected, tokens);
        }

        [Fact]
        public void Tokenize_EmptyString_ReturnsEmptyList()
        {
            var tokens = tokenizer.Tokenize("");

            Assert.Empty(tokens);
        }

        [Fact]
        public void Tokenize_WhitespaceOnly_ReturnsEmptyList()
        {
            var tokens = tokenizer.Tokenize("    ");

            Assert.Empty(tokens);
        }

        [Fact]
        public void Tokenize_PunctuationHandling_WorksCorrectly()
        {
            string input = "Wait... what?! Really?";
            var expected = new List<string> { "wait", "what", "really" };

            var tokens = tokenizer.Tokenize(input);

            Assert.Equal(expected, tokens);
        }
    }
}