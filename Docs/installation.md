# 🦙 Installing and Managing Ollama Models

> This guide walks you through setting up **Ollama** and downloading the language models required by **Gems AI**. These steps work on macOS, Linux, and Windows (via WSL).

---

## 1️⃣ Prerequisites

| Requirement | Minimum Version | Purpose |
|-------------|----------------|---------|
| **Docker** (optional) | 24.0 | Containerised Ollama runtime |
| **curl / Homebrew / apt / winget** | – | Package manager to install Ollama |
| **≈ 8 GB RAM** | – | Recommended for running 7‑B+ models locally |
| **Internet access** | – | To pull model weights from Ollama Hub |

---

## 2️⃣ Install the Ollama Runtime

### macOS

```bash
brew install ollama
```

### Ubuntu / Debian

```bash
curl -fsSL https://ollama.com/install.sh | sh
```

### Windows (WSL 2)

1. Install **Ubuntu 22.04** from the Microsoft Store.  
2. Open the Ubuntu terminal and run the Linux install script above.

> **Tip:** On native Windows, use Docker:
>
> ```powershell
> docker run -d --name ollama -p 11434:11434 ollama/ollama
> ```

---

## 3️⃣ Start the Ollama Service

```bash
ollama serve
# Service listens on http://localhost:11434
```

When running via Docker, the container exposes the same port `11434`.

---

## 4️⃣ Pull the Required Models

Gems AI dynamically selects the first available model. We recommend at least one 7‑B parameter model:

```bash
# Example: Llama 3 (8‑B)
ollama pull llama3

# Lightweight fallback (3‑B)
ollama pull gemma:2b
```

List installed models:

```bash
ollama list
```

---

## 5️⃣ Verify Connectivity

Run the test endpoint:

```bash
curl http://localhost:11434/api/generate -d '{
  "model": "llama3",
  "prompt": "Hello, world!",
  "stream": false
}'
```

You should receive a JSON response with a `response` field containing model output.

---

## 6️⃣ Configure Gems AI

Set the environment variable so Gems AI can reach Ollama:

```bash
export OLLAMA_HOST=http://localhost:11434
# (Windows PowerShell)
# $env:OLLAMA_HOST = "http://localhost:11434"
```

If you changed the port or are running in Docker on another host, update the URL accordingly.

---

## 7️⃣ Updating or Removing Models

### Update

```bash
ollama pull llama3 --update
```

### Remove

```bash
ollama rm llama3
```

---

## 8️⃣ Troubleshooting

| Symptom | Possible Cause | Fix |
|---------|----------------|-----|
| `connection refused` | Ollama service not running | Run `ollama serve` or start Docker container |
| `model not found` | Model not pulled | Run `ollama pull <model>` |
| High CPU/RAM usage | Large model size | Use a smaller model (e.g., `gemma:2b`) |

---

## 9️⃣ Next Steps

Return to the main **Gems AI** deployment guide and start the .NET `Runner`:

```bash
dotnet run --project Runner
```

Gems AI will query `/models`, detect your freshly installed model, and you’re ready to automate ERP tasks!

---
© 2024 Gems AI Documentation
