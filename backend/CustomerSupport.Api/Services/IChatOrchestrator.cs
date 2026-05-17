using CustomerSupport.Api.Orchestration;

namespace CustomerSupport.Api.Services;

public interface IChatOrchestrator
{
    ChatOrchestrationResult Analyze(string message);
}