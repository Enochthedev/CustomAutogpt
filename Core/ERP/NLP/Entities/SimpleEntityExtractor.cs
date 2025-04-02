using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace GemsAi.Core.ERP.NLP.Entities
{
    public class SimpleEntityExtractor
    {
        public Dictionary<string, string> Extract(string input)
        {
            var entities = new Dictionary<string, string>();

            // Extract name (assume first capitalized word after "Onboard" or "Hire")
            var nameMatch = Regex.Match(input, @"(?:Onboard|Hire)\s+([A-Z][a-z]+)");
            if (nameMatch.Success)
                entities["EmployeeName"] = nameMatch.Groups[1].Value;

            // Extract department (HR, Finance, IT, Sales)
            var departmentMatch = Regex.Match(input, @"\b(HR|Finance|IT|Sales)\b");
            if (departmentMatch.Success)
                entities["Department"] = departmentMatch.Value;

            // Extract date (YYYY-MM-DD)
            var dateMatch = Regex.Match(input, @"\d{4}-\d{2}-\d{2}");
            if (dateMatch.Success)
                entities["StartDate"] = dateMatch.Value;

            foreach (var entity in entities)
            {
                Console.WriteLine($"Extracted Entity - {entity.Key}: {entity.Value}");
            }

            return entities;
        }
    }
}