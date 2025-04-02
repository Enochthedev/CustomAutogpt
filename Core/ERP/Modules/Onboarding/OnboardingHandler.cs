using System;
using System.Collections.Generic;
using GemsAi.Core.ERP.Interfaces;

namespace GemsAi.Core.ERP.Modules.Onboarding
{
    public class OnboardingHandler : IModuleHandler
    {
        public string ModuleName => "Onboarding";

        public Dictionary<string, string> ParseInput(string input)
        {
            var data = new Dictionary<string, string>();
            // Example: "Onboard Sarah to HR on 2025-04-01"
            data["EmployeeName"] = ExtractValue(input, "([A-Z][a-z]+)");
            data["Department"] = ExtractValue(input, @"\b(HR|Finance|Sales|IT)\b");
            data["StartDate"] = ExtractValue(input, @"\d{4}-\d{2}-\d{2}");
            return data;
        }

        private string ExtractValue(string input, string pattern)
        {
            var match = System.Text.RegularExpressions.Regex.Match(input, pattern);
            return match.Success ? match.Value : "Unknown";
        }

        public void Execute(Dictionary<string, string> data)
        {
            Console.WriteLine("Executing Onboarding...");
            foreach (var entry in data)
            {
                Console.WriteLine($"{entry.Key}: {entry.Value}");
            }
        }
    }
}