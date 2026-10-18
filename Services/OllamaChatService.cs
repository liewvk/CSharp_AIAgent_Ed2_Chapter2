using System.Net.Http.Json;
using System.Text.Json;
using CSharpAIAgent.Memory;
using CSharpAIAgent.Models;

namespace CSharpAIAgent.Services;

public class OllamaChatService : IAiChatService, IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly ConversationMemory _memory;
    private readonly string _modelName;

    public OllamaChatService(
        string baseUrl,
        string modelName,
        ConversationMemory memory)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
            throw new ArgumentException(
                "Ollama server URL is required.");

        if (string.IsNullOrWhiteSpace(modelName))
            throw new ArgumentException(
                "Ollama model name is required.");

        _modelName = modelName;
        _memory = memory;

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromMinutes(10)
        };
    }

    public async Task<string> GetResponseAsync(
        string userInput)
    {
        if (string.IsNullOrWhiteSpace(userInput))
            throw new ArgumentException(
                "User input cannot be empty.");

        // Prepare the new user message.
        var userMessage = new ChatMessage(
            "user", userInput);

        // Include the previous conversation.
        var messages = _memory.GetMessages();

        // Include the current question in the request.
        messages.Add(userMessage);

        // Prepare the request for Ollama.
        var request = new
        {
            model = _modelName,

            messages = messages.Select(m => new
            {
                role = m.Role,
                content = m.Content
            }).ToArray(),

            stream = false,

            // Request the final answer directly.
            think = false
        };

        // Send the request to Ollama.
        using HttpResponseMessage response =
            await _httpClient.PostAsJsonAsync(
                "api/chat", request);

        // Read the API response.
        string responseText =
            await response.Content.ReadAsStringAsync();

        // Check for HTTP errors.
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"Ollama returned HTTP " +
                $"{(int)response.StatusCode}: " +
                responseText);
        }

        // Parse the JSON response.
        using JsonDocument json =
            JsonDocument.Parse(responseText);

        JsonElement message = json.RootElement
            .GetProperty("message");

        string answer = message
            .GetProperty("content")
            .GetString() ?? "";

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new InvalidOperationException(
                "Ollama returned an empty final answer.");
        }

        // Store messages only after a successful response.
        _memory.AddMessage("user", userInput);
        _memory.AddMessage("assistant", answer);

        return answer;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
