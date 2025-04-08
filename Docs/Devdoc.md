# 💻 Gems AI Agent – Developer Documentation

Welcome to the internal documentation for **Gems AI Agent**. This document provides technical insights, NuGet packages, and setup details to help contributors understand and extend the system efficiently.

---

## 🧠 Overview

Gems AI Agent is a modular .NET AI framework capable of:

- Dynamically creating and saving tasks as C# files.
- Routing prompts to the best available AI model using Ollama.
- Maintaining vector-based memory using either Ollama embeddings or ML.NET ONNX.
- Acting like an extendable autonomous assistant — capable of learning from user feedback.

---

## 📁 Project Structure

The solution is organized into several projects and folders:

```bash
GemsAi.sln
├── Core/
│   ├── Agent/               → Core logic of the AI agent
│   ├── AI/                  → Ollama API interaction and model routing
│   ├── Tasks/               → Built-in and custom commands (ITask-based)
│   ├── Memory/
│   │   ├── embedders/       → OllamaEmbedder and MLNetEmbedder
│   │   ├── IMemoryStore.cs
│   │   ├── InMemoryMemoryStore.cs
│   │   ├── IVectorMemoryStore.cs
│   │   └── InMemoryVectorMemoryStore.cs
│   └── LearnedTasks/
├── Runner/
│   └── Program.cs           → Entry point & DI bootstrapper
├── appsettings.json         → Configuration file
└── DEVELOPERS.md            → This file
```

---

## 📦 NuGet Packages Used

> Run these in the root solution directory, or target a project using `--project <path>`.

### ✅ Core Packages

```bash
dotnet add Core package Microsoft.ML
dotnet add Core package Microsoft.ML.OnnxTransformer
dotnet add Core package Microsoft.Extensions.Configuration
dotnet add Core package Microsoft.Extensions.Configuration.Json
```

### ✅ Runner (Console Host)

```bash
dotnet add Runner package Microsoft.Extensions.DependencyInjection
dotnet add Runner package Scrutor
dotnet add Runner package Microsoft.Extensions.Configuration
dotnet add Runner package Microsoft.Extensions.Configuration.Json
```

### ⚙️ Configuration via appsettings.json

The `appsettings.json` file is used to configure various aspects of the agent, including:

``` bash
{
  "Embedding": {
    "UseOllama": true,
    "OllamaEndpoint": "http://localhost:11434",
    "ONNXModelPath": "Assets/model.onnx"
  },
  "AI": {
    "DefaultModel": "smollm2:1.7b"
  },
  "Logging": {
    "LogLevel": "Information"
  }
}
```

### 🔧 Dependency Injection Setup (Runner/Program.cs)

Services are registered via ServiceCollection, including:
 • HttpClient
 • IMemoryStore & IVectorMemoryStore
 • IAIClient (Ollama)
 • IEmbedder (OllamaEmbedder or MLNetEmbedder)
 • ITask handlers (via Scrutor)
 • IAgent (GemsAgent)

Sample

```csharp
services.Scan(scan => scan
    .FromAssemblyOf<ITask>()
    .AddClasses(classes => classes.AssignableTo<ITask>())
    .AsImplementedInterfaces()
    .WithSingletonLifetime());
```

### 🧠 ML.NET + ONNX Embeddings

To use ML.NET as an embedder:
 • Install Microsoft.ML and Microsoft.ML.OnnxTransformer.
 • Download or convert an ONNX model (e.g., from HuggingFace or OpenAI).
 • Define an input/output schema like:

```csharp
public class EmbeddingInput { public string Text { get; set; } }
public class EmbeddingOutput { public float[] Embeddings { get; set; } }
```

 • Load and use it in MLNetEmbedder.

### 🧠 Ollama Embeddings (Alternative)

Ollama can also generate embeddings with models like nomic-embed-text. The OllamaEmbedder uses this via HTTP calls:

```csharp
POST http://localhost:11434/api/embeddings
{
  "model": "nomic-embed-text",
  "prompt": "your input text"
}
```

Your program can dynamically select and fallback between ML.NET and Ollama-based embedders.

### 🧠 Ollama API Client

The Ollama API client is a wrapper around the Ollama API, allowing you to interact with the models and embeddings easily. It abstracts the HTTP calls and provides a simple interface for generating text and embeddings.

```csharp
public class OllamaClient : IOllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;

    public OllamaClient(HttpClient httpClient, string endpoint)
    {
        _httpClient = httpClient;
        _endpoint = endpoint;
    }

    public async Task<string> GenerateText(string model, string prompt)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_endpoint}/api/generate", new { model, prompt });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
```

### 🧠 Task System

The task system is designed to be modular and extensible. Each task implements the ITask interface, which defines a method for executing the task. The GemsAgent class is responsible for dispatching tasks based on user input.

```csharp
public interface ITask
{
    string Name { get; }
    Task<string> Execute(string input);
}
```

### 🧠 Memory System

The memory system is designed to store and retrieve data using both key-value pairs and vector embeddings. The IMemoryStore interface defines methods for storing and retrieving data, while the IVectorMemoryStore interface extends this with vector-based storage.

```csharp
public interface IMemoryStore
{
    Task Store(string key, string value);
    Task<string> Retrieve(string key);
}
public interface IVectorMemoryStore : IMemoryStore
{
    Task StoreVector(string key, float[] vector);
    Task<float[]> RetrieveVector(string key);
}
```

### 🧠 Learned Tasks

The learned tasks system allows the agent to create and save new tasks dynamically. The LearnedTaskManager is responsible for loading and saving tasks, while the LearnedTaskMetadataManager records metadata about the tasks, such as the model used to create them.

```csharp
public class LearnedTaskManager
{
    private readonly string _taskDirectory;

    public LearnedTaskManager(string taskDirectory)
    {
        _taskDirectory = taskDirectory;
    }

    public async Task SaveTask(string taskName, string taskCode)
    {
        var filePath = Path.Combine(_taskDirectory, $"{taskName}.cs");
        await File.WriteAllTextAsync(filePath, taskCode);
    }

    public async Task<string> LoadTask(string taskName)
    {
        var filePath = Path.Combine(_taskDirectory, $"{taskName}.cs");
        return await File.ReadAllTextAsync(filePath);
    }
}
```

### 🧪 Running and Testing

From the root:

```bash
dotnet restore
dotnet build
dotnet run --project Runner
```

You should see:

```bash
🤖 Gems AI Agent Ready!
>
```

You can then type commands like:
  • echo hello – to test basic tasks.
  • what model are you using? – to check model status.
  • list models – to list available AI models.
  • create a task that reverses any string input from the user – to generate a new task.
  • what model did you use to create it? – to query metadata on generated tasks.

```bash
🤖 Gems AI Agent Ready!
> echo hello
hello
🤖 Gems AI Agent Ready!
> what model are you using?
🤖 I am using the smollm2:1.7b model.
```

### 🧩 Extending the Agent

Add a New Task

 1. Create a class in Core/Tasks/ that implements ITask.
 2. It will be auto-registered at runtime via Scrutor.

Add a New Embedder

 1. Implement IEmbedder.
 2. Inject and register it via config logic in Program.cs.

### 🛠️ Troubleshooting

- **Ollama API Issues**: Ensure the Ollama server is running and accessible at the specified endpoint.
- **Model Not Found**: Check if the model name is correct and available in your Ollama installation.
- **Embedding Errors**: Ensure the ONNX model path is correct and the model is compatible with ML.NET.
- **Task Creation Errors**: Ensure the task code is valid C# and adheres to the ITask interface.
- **Memory Issues**: Check if the memory store is correctly configured and accessible.
- **Logging**: Adjust the logging level in appsettings.json to get more detailed logs for debugging.
- **Configuration Issues**: Ensure appsettings.json is correctly formatted and all required fields are present.
- **Dependency Issues**: Ensure all required NuGet packages are installed and up to date.
- **Can't find interface** : Ensure the interface is in the correct namespace and the project is built.

### 🏁 Roadmap & Next Steps

 • Agent logic & Task routing
 • Task generation via code output
 • Model registry & routing
 • Embedder abstraction (Ollama & ML.NET)
 • Sandboxed long-term memory
 • Web interface & API hooks
 • Agent-to-agent communication
 • Plugin system

### 📝 License

This project is licensed under the MIT License. See the LICENSE file for details.

### ✨ Happy Building

If you have any questions or want to contribute improvements, feel free to create a PR or issue.

⸻

### Final Reminder: Run these if you haven’t already

```bash
dotnet add Core package Microsoft.ML
dotnet add Core package Microsoft.ML.OnnxTransformer
dotnet add Core package Microsoft.Extensions.Configuration
dotnet add Core package Microsoft.Extensions.Configuration.Json

dotnet add Runner package Microsoft.Extensions.DependencyInjection
dotnet add Runner package Scrutor
dotnet add Runner package Microsoft.Extensions.Configuration
dotnet add Runner package Microsoft.Extensions.Configuration.Json
```
