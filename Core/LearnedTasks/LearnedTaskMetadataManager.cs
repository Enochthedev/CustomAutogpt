using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace GemsAi.Core.LearnedTasks
{
    public static class LearnedTaskMetadataManager
    {
        private static readonly string MetadataFilePath = "Core/LearnedTasks/metadata.json";
        private static Dictionary<string, string> _metadata = new();

        public static async Task LoadMetadataAsync()
        {
            if (File.Exists(MetadataFilePath))
            {
                var json = await File.ReadAllTextAsync(MetadataFilePath);
                _metadata = JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
            }
            else
            {
                _metadata = new Dictionary<string, string>();
            }
        }

        public static async Task SaveMetadataAsync()
        {
            var json = JsonSerializer.Serialize(_metadata, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(MetadataFilePath, json);
        }

        public static async Task AddMetadataAsync(string taskFileName, string modelUsed)
        {
            _metadata[taskFileName] = modelUsed;
            await SaveMetadataAsync();
            // Also keep track of the last created task:
            LastCreatedTaskFile = taskFileName;
        }

        public static string? GetModelForTask(string taskFileName)
        {
            if (_metadata.TryGetValue(taskFileName, out var model))
                return model;
            return null;
        }

        public static string? LastCreatedTaskFile { get; private set; }
        public static string? GetLastCreatedTaskModel() =>
            LastCreatedTaskFile != null ? GetModelForTask(LastCreatedTaskFile) : null;
    }
}