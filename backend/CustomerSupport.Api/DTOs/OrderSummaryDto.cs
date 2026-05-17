namespace CustomerSupport.Api.DTOs;

public class OrderSummaryDto
{
    public int OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public int CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? Email { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ShippingStatus { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? PaymentMethod { get; set; }
    public string? TransactionId { get; set; }
    public string? CarrierName { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }
}