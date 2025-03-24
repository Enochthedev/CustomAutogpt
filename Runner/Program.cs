using GemsAi.Core.Agent;
using GemsAi.Core.LearnedTasks;
using GemsAi.Core.Memory;
using GemsAi.Core.Tasks;
using GemsAi.Core.AI;

var memory = new InMemoryMemoryStore();
try
{

    var httpClient = new HttpClient();
    var aiClient = new OllamaClient(httpClient); 
    var tasks = new List<ITask>
    {
        new EchoTask(),
        new StatusTask(aiClient),
        new ModelListTask(aiClient),
        new LLMTask(aiClient),
        new CreateTaskCommand(aiClient),
    };
    tasks.AddRange(LearnedTaskManager.LoadLearnedTasks());

    var agent = new ChronoAgent(tasks, memory);

    Console.WriteLine("🤖 Gems AI Agent Ready!");
    while (true)
    {
        Console.Write("> ");
        var input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) continue;

        var output = await agent.RunAsync(input);
        Console.WriteLine(output);
    }
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Error: {ex.Message}");
    Console.ResetColor();
}
