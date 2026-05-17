namespace CustomerSupport.Api.DTOs;

public class CustomerOverviewDto
{
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CustomerType { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? AccountStatus { get; set; }
    public int TotalOrders { get; set; }
    public decimal LifetimeSpend { get; set; }
    public DateTime? LastOrderDate { get; set; }
}