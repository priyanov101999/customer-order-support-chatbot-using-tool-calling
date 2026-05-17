namespace CustomerSupport.Api.DTOs;

public class ChatRequestDto
{
    public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = "default";
}