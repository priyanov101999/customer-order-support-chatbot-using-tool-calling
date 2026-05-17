using System.Collections.Concurrent;

namespace CustomerSupport.Api.Services;

public class InMemoryChatMemoryService : IChatMemoryService
{
    private readonly ConcurrentDictionary<string, List<string>> _history = new();

    public List<string> GetHistory(string sessionId)
    {
        return _history.GetOrAdd(sessionId, _ => new List<string>());
    }

    public void AddMessage(string sessionId, string role, string message)
    {
        var history = GetHistory(sessionId);

        history.Add($"{role}: {message}");

        if (history.Count > 10)
            history.RemoveAt(0);
    }
}