using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GemsAi.Core.Ai;
using GemsAi.Core.ERP.Models;
using GemsAi.Core.ERP.Interfaces;
using GemsAi.Core.ERP.Modules;  
using GemsAi.Core.ERP.NLP.Intents;


namespace GemsAi.Core.ERP.NLP
{
    public class NlpProcessor
    {
        private readonly IAiClient _aiClient;

        public NlpProcessor()
        {
            _aiClient = new OllamaClient(new HttpClient());
        }

        public async Task ProcessTextAsync(string input)
{
    try
    {
        Console.WriteLine("🔍 Detecting intent using AI...");
        string intent;
        
        try
        {
            intent = await _aiClient.DetectIntentAsync(input);
            if (string.IsNullOrEmpty(intent) || intent == "Unknown")
                throw new Exception("AI did not detect a valid intent.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❗ AI failed to detect intent ({ex.Message}), using rule-based detection...");
            intent = new IntentDetector().Detect(input);
        }

        Console.WriteLine($"✅ Detected Intent: {intent}");
        Console.WriteLine($"Is '{intent}' the correct module? (yes/no)");
        string confirmation = Console.ReadLine() ?? string.Empty;
        if (confirmation?.ToLower() != "yes")
        {
            Console.WriteLine("❌ Action cancelled.");
            return;
        }

        Console.WriteLine("🧠 Extracting entities using AI...");
        var entityData = await _aiClient.ExtractEntitiesAsync(input);

        var moduleData = new ModuleData(intent, "Create");
        foreach (var (key, value) in entityData)
        {
            moduleData.AddField(key, value);
        }

        Console.WriteLine("🔄 Confirm the extracted data:");
        moduleData.Display();

        Console.WriteLine("✅ Proceed with execution? (yes/no):");
        confirmation = Console.ReadLine() ?? string.Empty;
        if (confirmation?.ToLower() == "yes")
        {
            var handler = ModuleRegistry.GetHandler(intent);
            if (handler != null)
            {
                handler.Execute(moduleData.Fields); // Pass the dictionary from ModuleData
            }
            else
            {
                Console.WriteLine("❌ No suitable handler found for the detected intent.");
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
}}