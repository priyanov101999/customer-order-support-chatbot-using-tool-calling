namespace CustomerSupport.Api.DTOs;

public class CustomerOrderDto
{
    public int OrderId { get; set; }
    public string? OrderNumber { get; set; }
    public DateTime? OrderDate { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? ShippingStatus { get; set; }
    public decimal TotalAmount { get; set; }
}