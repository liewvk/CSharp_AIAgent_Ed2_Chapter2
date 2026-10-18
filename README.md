# C# AI Agent – Visual Studio 2026 and Ollama

A simple C# console application that connects to a local Ollama server and creates a lightweight AI assistant with conversation memory and a reusable chat service abstraction.

This project represents a practical example of building an AI-powered assistant in C# using:

- .NET console application
- Ollama local LLM integration
- Lightweight in-memory conversation history
- System prompt configuration
- Basic command-driven chat loop

## Project Overview

The application is designed to let a user talk to an AI model running locally through Ollama. It reads a configuration file, initializes a conversation memory, adds a system prompt, and sends each user message to the Ollama `/api/chat` endpoint.

The project is intentionally small and easy to follow, making it a good learning example for:

- calling external AI APIs from .NET
- structuring a minimal AI service layer
- keeping chat history between turns
- building prompt-driven assistants with a console UX

## Features

- Local AI chat using Ollama
- In-memory conversation memory
- Configurable Ollama base URL and model name
- Simple system prompt for a friendly C# tutor persona
- Command-based console app (`clear`, `count`, `exit`)
- Minimal and readable code structure for learning and experimentation

## Repository Structure

```text
CSharp_AIAgent_Ed2_Chapter2/
├── .gitattributes
├── .gitignore
├── appsettings.json
├── CSharpAIAgent.csproj
├── CSharpAIAgent.slnx
├── Program.cs
├── Memory/
│   └── ConversationMemory.cs
├── Models/
│   └── ChatMessage.cs
├── Prompts/
│   └── SystemPrompts.cs
├── Services/
│   ├── IAiChatService.cs
│   └── OllamaChatService.cs
└── README.md
```

## Key Components

### `Program.cs`
The entry point of the application.

It:

- loads configuration from `appsettings.json`
- creates a `ConversationMemory` instance
- adds the system prompt to memory
- creates an `OllamaChatService`
- runs a loop to accept user input
- supports commands for clearing chat history, showing count, and exiting

### `Services/IAiChatService.cs`
Defines the AI chat service contract.

```csharp
public interface IAiChatService
{
    Task<string> GetResponseAsync(string userInput);
}
```

This keeps the application flexible and allows for different backends in the future.

### `Services/OllamaChatService.cs`
The concrete implementation that calls Ollama.

Responsibilities:

- builds an HTTP client pointed at the Ollama server
- prepares a chat request with the selected model
- includes conversation history in the request
- posts the payload to `POST /api/chat`
- parses the response and returns the assistant message
- stores the user and assistant messages in memory after a successful call

### `Memory/ConversationMemory.cs`
A minimal in-memory store for chat messages.

It keeps a list of `ChatMessage` objects and exposes:

- `AddMessage(role, content)`
- `GetMessages()`
- `Clear()`
- `Count`

This is enough for simple conversational state without a database or external cache.

### `Models/ChatMessage.cs`
Represents one message in the conversation.

```csharp
public class ChatMessage
{
    public string Role { get; set; } = "";
    public string Content { get; set; } = "";
}
```

### `Prompts/SystemPrompts.cs`
Defines the system prompt shown to the model.

The app uses a friendly C# tutoring persona so the assistant behaves like a programming coach rather than a generic chatbot.

## Configuration

The application uses `appsettings.json`:

```json
{
  "OllamaSettings": {
    "BaseUrl": "http://localhost:11434/",
    "ModelName": "qwen3.5:9b"
  }
}
```

### Settings Explained

- `BaseUrl`: the local Ollama endpoint
- `ModelName`: which Ollama model to use

Before running the program, make sure Ollama is installed and the model is available locally.

## Prerequisites

You need:

- .NET 10 SDK (or a compatible preview environment)
- Visual Studio 2026 or a valid .NET CLI environment
- Ollama installed and running locally
- A downloaded model such as `qwen3.5:9b`

## Setup Instructions

### 1. Install and start Ollama

Follow the official Ollama installation instructions for your platform.

Then ensure the server is running locally:

```bash
ollama serve
```

### 2. Pull the model

```bash
ollama pull qwen3.5:9b
```

You can change the model name in `appsettings.json` if you prefer another model.

### 3. Restore and build the project

```bash
dotnet restore
```

```bash
dotnet build
```

### 4. Run the application

```bash
dotnet run
```

## Example Interaction

```text
======================================
       C# AI AGENT ASSISTANT
       Visual Studio 2026
       Powered by Ollama
======================================

AI Model: qwen3.5:9b
Ollama Server: http://localhost:11434/

Available Commands:
  clear  - Clear conversation history
  count  - Show stored message count
  exit   - Quit application

You: Explain dependency injection in C#

Qwen is thinking...

AI Assistant:
Dependency injection is a design pattern... 
```

## Available Commands

During the app session, these commands are supported:

- `clear` — clears the conversation history and re-adds the system prompt
- `count` — prints how many stored chat messages are currently in memory
- `exit` — closes the application

## How It Works

1. The app reads the Ollama settings from `appsettings.json`.
2. It creates a `ConversationMemory` object.
3. It adds the system prompt so the model stays in a tutoring persona.
4. For each user message:
   - the message is added to the current chat context
   - the app sends the conversation to Ollama
   - the returned assistant message is printed to the console
   - both the user message and the answer are stored in memory
5. The next turn includes this memory, so the model can respond in context.

## Notes and Design Choices

- This is a learning-focused project, not a production-ready chat platform.
- Conversation memory is held in RAM only; it is not persisted.
- The system prompt is intentionally simple and educational.
- The project uses plain `HttpClient` rather than a third-party SDK, which keeps the code easy to understand.
- The model and endpoint are configurable, making it simple to switch local runtimes.

## Good Next Improvements

Possible enhancements for future versions include:

- persistent conversation storage
- file-based logging
- streaming responses from Ollama
- support for multiple AI providers
- richer command handling
- authentication or API key integration for remote models
- structured responses and tool calling

## Summary

This repository is a compact and practical example of building a local AI assistant in C#. It demonstrates how to connect a .NET application to Ollama, maintain conversation context, and provide a simple interactive experience from the console.

It is ideal for developers who want to understand the fundamentals of AI agent design in C# without needing a large framework or heavy abstraction layer.

## License

This repository does not currently include a license file. If you plan to share or distribute the project publicly, consider adding an appropriate open-source license.
