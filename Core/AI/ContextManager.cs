using System.Collections.Generic;

public class ContextManager
{
    private readonly Queue<string> _conversationHistory;
    private const int MaxHistory = 5;

    public ContextManager()
    {
        _conversationHistory = new Queue<string>();
    }

    public void AddToHistory(string message)
    {
        if (_conversationHistory.Count >= MaxHistory)
        {
            _conversationHistory.Dequeue();
        }
        _conversationHistory.Enqueue(message);
    }

    public string GetContext()
    {
        return string.Join(" ", _conversationHistory);
    }
}