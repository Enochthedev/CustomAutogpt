using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;

namespace GemsAi.Core.ERP.NLP.Entities
{
    public class EntityExtractor
    {
        private readonly List<string> _departments;

        public EntityExtractor()
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

        public List<EntityData> ExtractEntities(string input)
        {
            var extractedEntities = new List<EntityData>();

            // Name Extraction (first capitalized word, allowing for first and last name)
            var nameMatch = Regex.Match(input, @"\b[A-Z][a-z]+(?:\s[A-Z][a-z]+)?\b");
            if (nameMatch.Success)
            {
                extractedEntities.Add(new EntityData
                {
                    EntityName = "EmployeeName",
                    EntityType = "Name",
                    EntityValue = nameMatch.Value
                });
            }

            // Department Extraction (from the loaded list)
            foreach (var dept in _departments)
            {
                if (input.Contains(dept, StringComparison.OrdinalIgnoreCase))
                {
                    extractedEntities.Add(new EntityData
                    {
                        EntityName = "Department",
                        EntityType = "Department",
                        EntityValue = dept
                    });
                    break;
                }
            }

            // Date Extraction (flexible format: YYYY-MM-DD, YYYY-M-D, DD-MM-YYYY, D-M-YYYY)
            var dateMatch = Regex.Match(input, @"\b(\d{4}-\d{1,2}-\d{1,2}|\d{2}-\d{2}-\d{4})\b");
            if (dateMatch.Success)
            {
                extractedEntities.Add(new EntityData
                {
                    EntityName = "StartDate",
                    EntityType = "Date",
                    EntityValue = dateMatch.Value
                });
            }

            return extractedEntities;
        }
    }
}