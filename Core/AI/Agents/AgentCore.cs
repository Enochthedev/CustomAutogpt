using System;
using System.Collections.Generic;
using AutoGPTDotNet.Core.Tasks;

namespace AutoGPTDotNet.Core.AI
{
    public class AgentCore : IAgentCore
    {
        private string _goal = string.Empty;
        private List<ITask> _tasks = new();

        public void SetGoal(string goal)
        {
            _goal = goal;
            Console.WriteLine($"[Agent] Goal Set: {goal}");
        }

        public void AddTask(ITask task)
        {
            _tasks.Add(task);
            Console.WriteLine($"[Agent] Task Added: {task.Description}");
        }

        public void ExecuteTasks()
        {
            Console.WriteLine($"[Agent] Executing {_tasks.Count} tasks...");

            foreach (var task in _tasks)
            {
                task.Execute();
            }

            Console.WriteLine("[Agent] All tasks completed.");
        }
    }
}