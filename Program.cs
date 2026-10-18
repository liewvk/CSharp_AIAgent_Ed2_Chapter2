using System.Text.Json;
using CSharpAIAgent.Memory;
using CSharpAIAgent.Prompts;
using CSharpAIAgent.Services;

Console.Title =
    "C# AI Agent - Visual Studio 2026 and Ollama";

Console.WriteLine("======================================");
Console.WriteLine("       C# AI AGENT ASSISTANT");
Console.WriteLine("       Visual Studio 2026");
Console.WriteLine("       Powered by Ollama");
Console.WriteLine("======================================");
Console.WriteLine();

try
{
    // Read application configuration.
    string configPath = Path.Combine(
        AppContext.BaseDirectory,
        "appsettings.json");

    string configText =
        File.ReadAllText(configPath);

    using JsonDocument config =
        JsonDocument.Parse(configText);

    JsonElement settings = config.RootElement
        .GetProperty("OllamaSettings");

    string baseUrl = settings
        .GetProperty("BaseUrl")
        .GetString() ?? "";

    string modelName = settings
        .GetProperty("ModelName")
        .GetString() ?? "";

    // Initialize conversation memory.
    var memory = new ConversationMemory();

    // Add the system prompt.
    memory.AddMessage(
        "system",
        SystemPrompts.BasicAssistantPrompt);

    // Create the Ollama AI service.
    using IAiChatServiceHolder holder =
        new IAiChatServiceHolder(
            new OllamaChatService(
                baseUrl, modelName, memory));

    IAiChatService aiService = holder.Service;

    Console.WriteLine($"AI Model: {modelName}");
    Console.WriteLine($"Ollama Server: {baseUrl}");
    Console.WriteLine();

    Console.WriteLine("Available Commands:");
    Console.WriteLine("  clear  - Clear conversation history");
    Console.WriteLine("  count  - Show stored message count");
    Console.WriteLine("  exit   - Quit application");
    Console.WriteLine();

    while (true)
    {
        Console.Write("You: ");

        string? userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput))
            continue;

        // Exit command.
        if (userInput.Equals(
            "exit",
            StringComparison.OrdinalIgnoreCase))
        {
            break;
        }

        // Clear conversation history.
        if (userInput.Equals(
            "clear",
            StringComparison.OrdinalIgnoreCase))
        {
            memory.Clear();

            memory.AddMessage(
                "system",
                SystemPrompts.BasicAssistantPrompt);

            Console.WriteLine(
                "Conversation history cleared.");
            Console.WriteLine();

            continue;
        }

        // Display message count.
        if (userInput.Equals(
            "count",
            StringComparison.OrdinalIgnoreCase))
        {
            int count = memory.Count - 1;

            Console.WriteLine(
                $"Stored conversation messages: {count}");
            Console.WriteLine();

            continue;
        }

        // Send the question to Qwen.
        try
        {
            Console.WriteLine();
            Console.WriteLine("Qwen is thinking...");

            string answer =
                await aiService.GetResponseAsync(userInput);

            Console.WriteLine();
            Console.WriteLine("AI Assistant:");
            Console.WriteLine(answer);
            Console.WriteLine();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine();
            Console.WriteLine("Ollama API error:");
            Console.WriteLine(ex.Message);
            Console.WriteLine();
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine();
            Console.WriteLine(
                "The AI request timed out.");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(
                "Error: " + ex.Message);
            Console.WriteLine();
        }
    }

    Console.WriteLine("Goodbye!");
}
catch (Exception ex)
{
    Console.WriteLine("Application startup failed:");
    Console.WriteLine(ex.Message);
}

// Helper class for managing service disposal.
public sealed class IAiChatServiceHolder : IDisposable
{
    public CSharpAIAgent.Services.IAiChatService Service
    {
        get;
    }

    public IAiChatServiceHolder(
        CSharpAIAgent.Services.OllamaChatService service)
    {
        Service = service;
    }

    public void Dispose()
    {
        if (Service is IDisposable disposable)
            disposable.Dispose();
    }
}
