using GemsAi.Core.AI;
using GemsAi.Core.Tasks;

namespace GemsAi.Core.Tasks
{
    public class CreateTaskCommand : ITask
    {
        private readonly IAIClient _ai;
        private readonly string _taskFolder = "Core/LearnedTasks";

        public CreateTaskCommand(IAIClient ai)
        {
            _ai = ai;

            // Ensure folder exists
            if (!Directory.Exists(_taskFolder))
                Directory.CreateDirectory(_taskFolder);
        }

        public bool CanHandle(string input) =>
            input.ToLower().StartsWith("create a task");

        public async Task<string> ExecuteAsync(string input)
        {
var prompt = $@"
You are an AI system helping build modular tasks for a .NET AI agent.
Given the user's request below, generate a C# class that implements the ITask interface.
- The class should have a clear `CanHandle()` and `ExecuteAsync()`
- It must be self-contained and compile-ready.
- It should go into the namespace `GemsAi.Core.Tasks`
- Do NOT add extra explanation — only give the code.

User request:
\""{input}\""
";

            var generatedCode = await _ai.GenerateAsync(prompt);

            // Try to extract class name
            var className = ExtractClassName(generatedCode);
            if (string.IsNullOrWhiteSpace(className))
                return "I generated code but couldn't find a class name. Task not saved.";

            var filePath = Path.Combine(_taskFolder, $"{className}.cs");
            await File.WriteAllTextAsync(filePath, generatedCode);

            return $"✅ I created and saved `{className}.cs`. Restart me to activate it!";
        }

        private string? ExtractClassName(string code)
        {
            var lines = code.Split('\n');
            foreach (var line in lines)
            {
                if (line.Trim().StartsWith("public class"))
                {
                    var parts = line.Trim().Split(' ');
                    return parts.Length >= 3 ? parts[2] : null;
                }
            }
            return null;
        }
    }
}