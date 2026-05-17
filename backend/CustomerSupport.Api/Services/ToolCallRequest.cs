namespace CustomerSupport.Api.Orchestration;

public class ToolCallRequest
{
    public string ToolName { get; set; } = "";

    public int? OrderId { get; set; } // matches table type

    public int? CustomerId { get; set; } // matches table type

    public string? SearchText { get; set; }
}