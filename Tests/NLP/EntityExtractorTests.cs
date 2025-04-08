using Xunit;
using System.Collections.Generic;
using GemsAI.Core.NLP.EntityExtraction;

namespace GemsAI.Tests.NLP
{
    public class EntityExtractorTests
    {
        private readonly EntityExtractor extractor;

        public EntityExtractorTests()
        {
            extractor = new EntityExtractor();
        }

        [Fact]
        public void TestEntityExtraction_NameAndDepartment()
        {
            string input = "Add John Doe to the HR department.";
            var result = extractor.ExtractEntities(input);

            Assert.True(result.ContainsKey("Name"));
            Assert.Equal("John Doe", result["Name"]);
            Assert.True(result.ContainsKey("Department"));
            Assert.Equal("HR", result["Department"]);
        }

        [Fact]
        public void TestEntityExtraction_OnlyName()
        {
            string input = "Hello Jane Smith.";
            var result = extractor.ExtractEntities(input);

            Assert.True(result.ContainsKey("Name"));
            Assert.Equal("Jane Smith", result["Name"]);
            Assert.False(result.ContainsKey("Department"));
        }

        [Fact]
        public void TestEntityExtraction_OnlyDepartment()
        {
            string input = "Transfer to Finance department.";
            var result = extractor.ExtractEntities(input);

            Assert.False(result.ContainsKey("Name"));
            Assert.True(result.ContainsKey("Department"));
            Assert.Equal("Finance", result["Department"]);
        }

        [Fact]
        public void TestEntityExtraction_NoMatch()
        {
            string input = "This is a random sentence.";
            var result = extractor.ExtractEntities(input);

            Assert.Empty(result);
        }

        [Fact]
        public void TestEntityExtraction_NullInput()
        {
            string input = string.Empty;
            var result = extractor.ExtractEntities(input);

            Assert.Empty(result);
        }

        [Fact]
        public void TestEntityExtraction_EmptyInput()
        {
            string input = "";
            var result = extractor.ExtractEntities(input);

            Assert.Empty(result);
        }

        [Fact]
        public void TestEntityExtraction_MalformedPattern()
        {
            string input = "Add a person without a clear pattern.";
            var result = extractor.ExtractEntities(input);

            Assert.Empty(result);
        }

        [Fact]
        public void TestEntityExtraction_CommonWordPrefix()
        {
            string input = "Hey John Doe, welcome to Marketing!";
            var result = extractor.ExtractEntities(input);

            Assert.True(result.ContainsKey("Name"));
            Assert.Equal("John Doe", result["Name"]);
            Assert.True(result.ContainsKey("Department"));
            Assert.Equal("Marketing", result["Department"]);
        }
    }
}