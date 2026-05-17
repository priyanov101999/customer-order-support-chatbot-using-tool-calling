namespace CustomerSupport.Api.DTOs;

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string? CustomerCode { get; set; }
    public string? CustomerName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? CustomerType { get; set; }
    public int LoyaltyPoints { get; set; }
    public string? AccountStatus { get; set; }
    public bool IsActive { get; set; }
}