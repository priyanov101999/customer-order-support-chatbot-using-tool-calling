namespace CustomerSupport.Api.Services;

public interface IChatMemoryService
{
    List<string> GetHistory(string sessionId);
    void AddMessage(string sessionId, string role, string message);
}