using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using GemsAi.Core.ERP.NLP.Intents;
using GemsAi.Core.ERP.NLP.Entities;

namespace GemsAi.Core.ERP.NLP
{
    public class SimpleNlpProcessor
    {
        public async Task ProcessTextAsync(string input)
        {
            try
            {
                Console.WriteLine("🔍 Detecting intent...");
                var intentDetector = new SimpleIntentDetector();
                string intent = intentDetector.Detect(input);

                if (intent == "Unknown")
                {
                    Console.WriteLine("❌ Could not detect intent. Please try again.");
                    return;
                }

                Console.WriteLine($"✅ Detected Intent: {intent}");

                Console.WriteLine("🧠 Extracting entities...");
                var entityExtractor = new SimpleEntityExtractor();
                var entityData = entityExtractor.Extract(input);

                if (entityData.Count == 0)
                {
                    Console.WriteLine("❌ No entities detected. Please check your input.");
                    return;
                }

                while (true)
                {
                    Console.WriteLine("\n🔄 Confirm the extracted data:");
                    foreach (var (key, value) in entityData)
                    {
                        Console.WriteLine($"{key}: {value}");
                    }

                    Console.WriteLine("\n🔧 Do you want to change any field? (yes/no):");
                    string change = Console.ReadLine()?.ToLower();

                    if (change == "yes")
                    {
                        Console.Write("Enter the field name to change: ");
                        string field = Console.ReadLine();

                        if (field != null && entityData.ContainsKey(field))
                        {
                            Console.Write($"Enter new value for {field}: ");
                            string newValue = Console.ReadLine();

                            if (!string.IsNullOrWhiteSpace(newValue))
                            {
                                entityData[field] = newValue;
                                Console.WriteLine($"✅ Updated {field} to {newValue}");
                            }
                            else
                            {
                                Console.WriteLine("❗ Invalid value. Field not changed.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("❗ Field not found. Please try again.");
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                Console.WriteLine("\n✅ Proceed with execution? (yes/no):");
                string confirmation = Console.ReadLine()?.ToLower();
                if (confirmation == "yes")
                {
                    Console.WriteLine("🎉 Action executed with the following data:");
                    foreach (var (key, value) in entityData)
                    {
                        Console.WriteLine($"{key}: {value}");
                    }
                }
                else
                {
                    Console.WriteLine("❌ Action cancelled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"🛑 Error during NLP processing: {ex.Message}");
            }
        }
    }
}