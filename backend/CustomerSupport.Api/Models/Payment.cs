namespace CustomerSupport.Api.Models;

public class Payment
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public string? PaymentReference { get; set; }

    public string? PaymentMethod { get; set; }

    public string? PaymentStatus { get; set; }

    public decimal Amount { get; set; }

    public string? Currency { get; set; }

    public string? TransactionId { get; set; }

    public string? GatewayName { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? FailureReason { get; set; }

    public string? RefundStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}