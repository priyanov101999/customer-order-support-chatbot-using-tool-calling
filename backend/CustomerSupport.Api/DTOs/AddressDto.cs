namespace CustomerSupport.Api.DTOs;

public class AddressDto
{
    public int AddressId { get; set; }
    public int CustomerId { get; set; }
    public string? AddressType { get; set; }
    public string? Line1 { get; set; }
    public string? Line2 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public bool IsDefaultShipping { get; set; }
    public bool IsDefaultBilling { get; set; }
}