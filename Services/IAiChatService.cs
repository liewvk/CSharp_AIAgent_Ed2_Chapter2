namespace CSharpAIAgent.Services;

public interface IAiChatService
{
    Task<string> GetResponseAsync(string userInput);
}
