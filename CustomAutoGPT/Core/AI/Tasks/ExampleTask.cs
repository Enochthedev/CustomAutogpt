using System;

namespace AutoGPTDotNet.Core.Tasks
{
    public class ExampleTask : ITask
    {
        public string Description => "Sample task execution.";

        public void Execute()
        {
            Console.WriteLine("[Task] Executing Example Task...");
        }
    }
}