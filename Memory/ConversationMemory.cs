using CSharpAIAgent.Models;

namespace CSharpAIAgent.Memory;

public class ConversationMemory
{
    private readonly List<ChatMessage> _messages = new();

    public void AddMessage(string role, string content)
    {
        _messages.Add(new ChatMessage(role, content));
    }

    public List<ChatMessage> GetMessages()
    {
        return new List<ChatMessage>(_messages);
    }

    public void Clear()
    {
        _messages.Clear();
    }

    public int Count => _messages.Count;
}