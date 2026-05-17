namespace CustomerSupport.Api.DTOs;

public class ProductDto
{
    public int ProductId { get; set; }
    public string? ProductCode { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public string? ProductStatus { get; set; }
    public string? SupplierName { get; set; }
}