namespace CustomerSupport.Api.DTOs;

public class PendingRefundDto
{
    public int ReturnId { get; set; }
    public int OrderId { get; set; }
    public string? ReturnReference { get; set; }
    public decimal RefundAmount { get; set; }
    public string? RefundStatus { get; set; }
    public DateTime? RequestedDate { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? Email { get; set; }
}