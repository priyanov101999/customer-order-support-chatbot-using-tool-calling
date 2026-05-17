using CustomerSupport.Api.Orchestration;

namespace CustomerSupport.Api.Services;

public interface ILlmChatOrchestrator
{
    Task<LlmToolDecision> DecideAsync(string userMessage, List<string> history);
    Task<string> GenerateFinalResponseAsync(string userMessage, object toolResults);
    
}