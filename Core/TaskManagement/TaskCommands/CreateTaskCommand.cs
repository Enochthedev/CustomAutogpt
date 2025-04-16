using GemsAi.Core.Ai;
using GemsAi.Core.LearnedTasks;
using GemsAi.Core.Tasks;
using GemsAi.Core.TaskManagement.TaskCommands;

namespace GemsAi.Core.Tasks
{
    public class CreateTaskCommand : ITask
    {
        private readonly IAiClient _ai;
        private readonly string _taskFolder = "Core/LearnedTasks";

        public CreateTaskCommand(IAiClient ai)
        {
            _ai = ai;
            if (!Directory.Exists(_taskFolder))
                Directory.CreateDirectory(_taskFolder);
        }

        public bool CanHandle(string input) =>
            input.ToLower().StartsWith("create a task");

        public async Task<string> ExecuteAsync(string input)
        {
            var basePrompt = $@"
            You are an Ai system helping build modular tasks for a .NET Ai agent.
            Given the user's request below, generate a C# class that implements the ITask interface.
            - The class should have clear implementations for CanHandle() and ExecuteAsync().
            - It must be self-contained and compile-ready.
            - It should be in the namespace GemsAi.Core.Tasks.
            - Do NOT add any extra explanation — only output valid C# code.

            User request:
            """"""{input}""""""";

                string prompt = basePrompt;
                string generatedCode = "";
                const int maxIterations = 3;
                int iteration = 0;

                while (iteration < maxIterations)
                {
                    // Generate code with the current prompt, using model routing for code generation.
                    var availableModels = await _ai.GetAllModelsAsync();
                    var router = new ModelRouter(availableModels);
                    var bestModel = router.PickBestModel("code generation");
                    generatedCode = await _ai.GenerateAsync(prompt);

                    Console.WriteLine("Generated Code:");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine(generatedCode);
                    Console.WriteLine("-----------------------------------");
                    Console.Write("Is this code acceptable? (y to accept, n to refine): ");
                    var confirmation = Console.ReadLine();
                    if (confirmation?.Trim().ToLower() == "y")
                    {
                        break;
                    }
                    else
                    {
                        Console.Write("Please describe the errors or issues observed: ");
                        var errorFeedback = Console.ReadLine() ?? "";
                        // Append user feedback to the base prompt for refinement.
                        prompt = basePrompt + "\n" +
                                "The previously generated code had the following issues:\n" +
                                errorFeedback + "\n" +
                                "Please generate a revised version of the C# class that implements ITask and compiles without errors.";
                        iteration++;
                    }
                }

            if (iteration == maxIterations && string.IsNullOrWhiteSpace(generatedCode))
            {
                return "Task creation aborted after multiple iterations.";
            }

            // Try to extract the class name
            var className = ExtractClassName(generatedCode);
            if (string.IsNullOrWhiteSpace(className))
                return "I generated code but couldn't find a class name. Task not saved.";

            var filePath = Path.Combine(_taskFolder, $"{className}.cs");
            await File.WriteAllTextAsync(filePath, generatedCode);

            // Record metadata for the generated task
            var availableModelsFinal = await _ai.GetAllModelsAsync();
            var routerFinal = new ModelRouter(availableModelsFinal);
            var finalModel = routerFinal.PickBestModel("code generation");
            await LearnedTaskMetadataManager.AddMetadataAsync($"{className}.cs", finalModel);

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