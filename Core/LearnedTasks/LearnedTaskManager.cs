using System.Reflection;
using System.Runtime.Loader;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using GemsAi.Core.Tasks;

namespace GemsAi.Core.LearnedTasks
{
    public static class LearnedTaskManager
    {
        public static List<ITask> LoadLearnedTasks()
        {
            var tasks = new List<ITask>();
            var taskFiles = Directory.GetFiles("Core/LearnedTasks", "*.cs");

            foreach (var file in taskFiles)
            {
                var sourceCode = File.ReadAllText(file);

                var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
                var references = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
                    .Select(a => MetadataReference.CreateFromFile(a.Location))
                    .Cast<MetadataReference>();

                var compilation = CSharpCompilation.Create(
                    $"LearnedTask_{Path.GetFileNameWithoutExtension(file)}",
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                );

                using var ms = new MemoryStream();
                var result = compilation.Emit(ms);
                if (!result.Success) continue;

                ms.Seek(0, SeekOrigin.Begin);
                var assembly = AssemblyLoadContext.Default.LoadFromStream(ms);

                var taskTypes = assembly.GetTypes()
                    .Where(t => typeof(ITask).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var type in taskTypes)
                {
                    if (Activator.CreateInstance(type) is ITask task)
                    {
                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }
    }
}