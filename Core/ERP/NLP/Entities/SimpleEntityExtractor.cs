using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace GemsAi.Core.ERP.NLP.Entities
{
    public class SimpleEntityExtractor
    {
        private readonly List<string> _departments;

        public SimpleEntityExtractor()
        {
            _departments = LoadDepartments();
        }

        private List<string> LoadDepartments()
        {
            try
            {
                string filePath = Path.Combine("Core", "ERP", "NLP", "Departments.json");
                string jsonData = File.ReadAllText(filePath);
                var departmentData = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData);
                return departmentData?["Departments"] ?? new List<string> { "HR", "Finance", "Sales", "IT" };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading departments: {ex.Message}");
                return new List<string> { "HR", "Finance", "Sales", "IT" };
            }
        }

        public Dictionary<string, string> Extract(string input)
        {
            var entities = new Dictionary<string, string>();

            // Extract name (any capitalized word after onboarding keywords)
            var nameMatch = Regex.Match(input, @"(?:Onboard|Hire|Add)\s+([A-Z][a-z]+(?:\s[A-Z][a-z]+)?)");
            if (nameMatch.Success)
                entities["EmployeeName"] = nameMatch.Groups[1].Value;

            // Extract department from a dynamic list
            foreach (var dept in _departments)
            {
                if (input.Contains(dept, StringComparison.OrdinalIgnoreCase))
                {
                    entities["Department"] = dept;
                    break;
                }
            }

            // Extract date (more flexible format)
            var dateMatch = Regex.Match(input, @"\b(\d{4}-\d{1,2}-\d{1,2}|\d{1,2}-\d{1,2}-\d{4})\b");
            if (dateMatch.Success)
                entities["StartDate"] = dateMatch.Value;

            // Print extracted entities
            foreach (var entity in entities)
            {
                Console.WriteLine($"Extracted Entity - {entity.Key}: {entity.Value}");
            }

            return entities;
        }
    }
}