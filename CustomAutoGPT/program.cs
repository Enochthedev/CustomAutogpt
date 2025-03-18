using System;
using AutoGPTDotNet.Core.AI;
using AutoGPTDotNet.Core.Tasks;

namespace AutoGPTDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[Test] Initializing Agent Core...");

            // Create an instance of the agent
            IAgentCore agent = new AgentCore();
            agent.SetGoal("Test AutoGPT Execution");

            // Create and add a sample task
            ITask task = new ExampleTask();
            agent.AddTask(task);

            // Execute tasks
            agent.ExecuteTasks();

            Console.WriteLine("[Test] Task execution completed.");
        }
    }
}