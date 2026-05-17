namespace CustomerSupport.Api.DTOs;

public class ChatResponseDto
{
    public string ToolUsed { get; set; } = string.Empty;
    public object? Data { get; set; }
    public string Message { get; set; } = string.Empty;
}