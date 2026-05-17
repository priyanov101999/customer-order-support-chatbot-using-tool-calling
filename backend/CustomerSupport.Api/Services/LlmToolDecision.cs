namespace CustomerSupport.Api.Orchestration;

public class LlmToolDecision
{
    public string Action { get; set; } = "";
    public string FollowUpQuestion { get; set; } = "";
    public List<ToolCallRequest> Tools { get; set; } = new();
}