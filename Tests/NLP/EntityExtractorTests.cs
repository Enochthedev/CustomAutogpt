using Xunit;
using Core.InProg.NLP.EntityExtraction;
using System.Linq;

namespace GemsAI.Tests.INProg.NLP
{
    public class EntityExtractorTests
    {
        private readonly EntityExtractor _extractor;

        public EntityExtractorTests()
        {
            _extractor = new EntityExtractor("Assets/NLP/EntitySchema.json");
        }

        [Fact]
        public void ExtractEntities_ValidSentence_ExtractsExpectedEntities()
        {
            var result = _extractor.ExtractEntities("Onboard Sarah Doe to the HR department.");

            Assert.Contains(result, e => e.Entity == "Sarah" && e.Type == "name");
            Assert.Contains(result, e => e.Entity == "HR" && e.Type == "department");
        }

        [Fact]
        public void ExtractEntities_NoMatch_ReturnsEmptyList()
        {
            var result = _extractor.ExtractEntities("This is a generic sentence.");
            Assert.Empty(result);
        }
    }
}