namespace CustomerSupport.Api.Orchestration;

public class ChatOrchestrationResult
{
    public ChatIntent Intent { get; set; }
    public int? OrderId { get; set; }
    public int? CustomerId { get; set; }
    public bool NeedsMoreInfo { get; set; }
    public string? FollowUpQuestion { get; set; }
}