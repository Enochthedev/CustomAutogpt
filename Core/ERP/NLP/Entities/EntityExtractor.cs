using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GemsAi.Core.ERP.NLP.Entities
{
    public class EntityExtractor
    {
        public List<EntityData> ExtractEntities(string input)
        {
            var extractedEntities = new List<EntityData>();

            foreach (var entityType in EntityDefinitions.EntityPatterns)
            {
                var matches = Regex.Matches(input, entityType.Value, RegexOptions.IgnoreCase);
                foreach (Match match in matches)
                {
                    extractedEntities.Add(new EntityData
                    {
                        EntityName = match.Value,
                        EntityType = entityType.Key,
                        EntityValue = match.Value
                    });
                }
            }

            return extractedEntities;
        }
    }
}