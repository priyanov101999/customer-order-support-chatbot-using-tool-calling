namespace CustomerSupport.Api.DTOs;

public class ReturnStatusDto
{
    public int ReturnId { get; set; }
    public int OrderId { get; set; }
    public string? ReturnReference { get; set; }
    public string? ReturnReason { get; set; }
    public string? ReturnStatus { get; set; }
    public decimal RefundAmount { get; set; }
    public string? RefundStatus { get; set; }
    public DateTime? RequestedDate { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public DateTime? RefundedDate { get; set; }
    public string? CustomerComments { get; set; }
    public string? InternalNotes { get; set; }
}