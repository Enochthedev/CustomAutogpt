using System;
using System.Collections.Generic;

namespace GemsAi.Core.ERP.NLP.Intents
{
    public class SimpleIntentDetector
    {
        private readonly Dictionary<string, string> _intentMap = new()
        {
            { "onboard", "Onboarding" },
            { "hire", "Onboarding" },
            { "add employee", "Onboarding" },
            { "update employee", "Onboarding" },
            { "generate payroll", "Payroll" },
            { "update salary", "Payroll" },
            { "payroll report", "Payroll" }
        };

        public string Detect(string input)
        {
            foreach (var entry in _intentMap)
            {
                if (input.Contains(entry.Key, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine($"Detected intent '{entry.Value}' from keyword '{entry.Key}'");
                    return entry.Value;
                }
            }

            Console.WriteLine("No matching intent found. Defaulting to 'Unknown'.");
            return "Unknown";
        }
    }
}