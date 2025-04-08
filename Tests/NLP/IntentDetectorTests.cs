using Xunit;
using System.Collections.Generic;
using GemsAI.Core.NLP.IntentDetection;

namespace GemsAI.Tests.NLP
{
    public class IntentDetectorTests
    {
        private readonly IntentDetector intentDetector;

        public IntentDetectorTests()
        {
            intentDetector = new IntentDetector();
        }

        [Fact]
        public void TestIntentDetection_Onboarding()
        {
            string input = "Onboard John Doe to the HR department.";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("onboarding", result);
        }

        [Fact]
        public void TestIntentDetection_Payroll()
        {
            string input = "Process payroll for the finance department.";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("payroll", result);
        }

        [Fact]
        public void TestIntentDetection_UnknownIntent()
        {
            string input = "This is a random sentence without intent.";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("unknown", result);
        }

        [Fact]
        public void TestIntentDetection_NullInput()
        {
            string input = string.Empty;
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("unknown", result);
        }

        [Fact]
        public void TestIntentDetection_EmptyInput()
        {
            string input = "";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("unknown", result);
        }

        [Fact]
        public void TestIntentDetection_MultipleValidIntents()
        {
            string input = "Onboard John and process payroll simultaneously.";
            var result = intentDetector.DetectIntent(input);

            // Test should assert the primary intent if multiple are found
            Assert.Equal("onboarding", result);
        }

        [Fact]
        public void TestIntentDetection_EdgeCase_WeirdInput()
        {
            string input = "!!@# Onboard!@ John 123 to **Payroll**!";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("onboarding", result);
        }

        [Fact]
        public void TestIntentDetection_CaseInsensitive()
        {
            string input = "onBoard John Doe to THE hr department.";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("onboarding", result);
        }

        [Fact]
        public void TestIntentDetection_SpecialCharacters()
        {
            string input = "Process payroll! Urgently.";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("payroll", result);
        }

        [Fact]
        public void TestIntentDetection_MixedLanguage()
        {
            string input = "Onboard João into HR.";
            var result = intentDetector.DetectIntent(input);

            Assert.Equal("onboarding", result);
        }
    }
}