using Xunit;
using Core.InProg.NLP.IntentDetection;

namespace GemsAI.Tests.INProg.NLP
{
    public class IntentDetectorTests
    {
        private readonly IntentDetector _intentDetector;

        public IntentDetectorTests()
        {
            _intentDetector = new IntentDetector("Assets/NLP/IntentPatterns.json");
        }

        [Theory]
        [InlineData("Onboard John Doe as a developer", "onboard")]
        [InlineData("Can you move Sarah to HR?", "move")]
        [InlineData("Please delete James from payroll", "delete")]
        public void DetectIntent_KnownPatterns_ReturnsExpectedIntent(string input, string expected)
        {
            var result = _intentDetector.DetectIntent(input);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void DetectIntent_UnknownIntent_ReturnsUnknown()
        {
            var result = _intentDetector.DetectIntent("This is an unrelated sentence");
            Assert.Equal("unknown", result);
        }
    }
}