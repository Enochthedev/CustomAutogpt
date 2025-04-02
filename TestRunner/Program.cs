using System;
using System.Threading.Tasks;
using GemsAi.Core.ERP.NLP;

namespace TestRunner
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("🚀 AI Test Runner - Demonstration Mode");
            var nlpProcessor = new SimpleNlpProcessor();

            while (true)
            {
                Console.Write("\n> ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) continue;

                await nlpProcessor.ProcessTextAsync(input);
            }
        }
    }
}