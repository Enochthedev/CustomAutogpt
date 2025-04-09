namespace GemsAi.Core.TaskManagement.TaskCommands.Utils
{
    public class ErpModuleSchema
    {
        public List<string> RequiredFields { get; set; } = new();
        public Dictionary<string, string> ExampleFormat { get; set; } = new();
        public List<string>? AvailableEndpoints { get; set; } = new(); // Optional
    }
}