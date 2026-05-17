// Repository layer 
using System.Data;
using CustomerSupport.Api.Data;
using CustomerSupport.Api.DTOs;
using Dapper;

namespace CustomerSupport.Api.Repositories;

public class CustomerSupportRepository : ICustomerSupportRepository
{
    private readonly DatabaseContext _context;

    public CustomerSupportRepository(DatabaseContext context)
    {
        _context = context;
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                CustomerId,
                CustomerCode,
                CONCAT(FirstName, ' ', LastName) AS CustomerName,
                Email,
                Phone,
                CustomerType,
                LoyaltyPoints,
                AccountStatus,
                IsActive
            FROM Customers
            WHERE CustomerId = @CustomerId;
            """;

        return await connection.QueryFirstOrDefaultAsync<CustomerDto>(
            sql,
            new { CustomerId = customerId }
        );
    }

    public async Task<IReadOnlyList<AddressDto>> GetCustomerAddressesAsync(int customerId)
    {
        using var connection = _context.CreateConnection();

        const string sql = """
            SELECT
                AddressId,
                CustomerId,
                AddressType,
                Line1,
                Line2,
                City,
                State,
                PostalCode,
                Country,
                IsDefaultShipping,
                IsDefaultBilling
            FROM Addresses
            WHERE CustomerId = @CustomerId
              AND IsActive = TRUE;
            """;

        var result = await connection.QueryAsync<AddressDto>(
            sql,
            new { CustomerId = customerId }
        );

        return result.ToList();
    }

    public async Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string searchText)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ProductDto>(
            "sp_SearchProducts",
            new { p_SearchText = searchText },
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }

    public async Task<OrderSummaryDto?> GetOrderSummaryAsync(int orderId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<OrderSummaryDto>(
            "sp_GetOrderSummary",
            new { p_OrderId = orderId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IReadOnlyList<CustomerOrderDto>> GetCustomerOrderHistoryAsync(int customerId)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<CustomerOrderDto>(
            "sp_GetCustomerOrderHistory",
            new { p_CustomerId = customerId },
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }

    public async Task<PaymentStatusDto?> GetPaymentStatusAsync(int orderId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<PaymentStatusDto>(
            "sp_GetPaymentStatus",
            new { p_OrderId = orderId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<ShipmentStatusDto?> GetShipmentStatusAsync(int orderId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ShipmentStatusDto>(
            "sp_GetShipmentStatus",
            new { p_OrderId = orderId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<ReturnStatusDto?> GetReturnStatusAsync(int orderId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<ReturnStatusDto>(
            "sp_GetReturnStatus",
            new { p_OrderId = orderId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<CustomerOverviewDto?> GetCustomerOverviewAsync(int customerId)
    {
        using var connection = _context.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<CustomerOverviewDto>(
            "sp_GetCustomerOverview",
            new { p_CustomerId = customerId },
            commandType: CommandType.StoredProcedure
        );
    }

    public async Task<IReadOnlyList<PendingRefundDto>> GetPendingRefundsAsync()
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<PendingRefundDto>(
            "sp_GetPendingRefunds",
            commandType: CommandType.StoredProcedure
        );

        return result.ToList();
    }
    public async Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            CustomerId,
            CustomerCode,
            CONCAT(FirstName, ' ', LastName) AS CustomerName,
            Email,
            Phone,
            CustomerType,
            LoyaltyPoints,
            AccountStatus,
            IsActive
        FROM Customers;
        """;

    var result = await connection.QueryAsync<CustomerDto>(sql);
    return result.ToList();
}

public async Task<IReadOnlyList<AddressDto>> GetAllAddressesAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            AddressId,
            CustomerId,
            AddressType,
            Line1,
            Line2,
            City,
            State,
            PostalCode,
            Country,
            IsDefaultShipping,
            IsDefaultBilling
        FROM Addresses;
        """;

    var result = await connection.QueryAsync<AddressDto>(sql);
    return result.ToList();
}

public async Task<IReadOnlyList<ProductDto>> GetAllProductsAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            ProductId,
            ProductCode,
            ProductName,
            Category,
            Brand,
            UnitPrice,
            StockQuantity,
            ProductStatus,
            SupplierName
        FROM Products;
        """;

    var result = await connection.QueryAsync<ProductDto>(sql);
    return result.ToList();
}

public async Task<IReadOnlyList<OrderSummaryDto>> GetAllOrdersAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            o.OrderId,
            o.OrderNumber,
            c.CustomerId,
            CONCAT(c.FirstName, ' ', c.LastName) AS CustomerName,
            c.Email,
            o.OrderDate,
            o.OrderStatus,
            o.PaymentStatus,
            o.ShippingStatus,
            o.SubTotal,
            o.TaxAmount,
            o.DiscountAmount,
            o.TotalAmount,
            p.PaymentMethod,
            p.TransactionId,
            s.CarrierName,
            s.TrackingNumber,
            s.EstimatedDeliveryDate
        FROM Orders o
        JOIN Customers c ON o.CustomerId = c.CustomerId
        LEFT JOIN Payments p ON o.OrderId = p.OrderId
        LEFT JOIN Shipments s ON o.OrderId = s.OrderId;
        """;

    var result = await connection.QueryAsync<OrderSummaryDto>(sql);
    return result.ToList();
}

public async Task<IReadOnlyList<PaymentStatusDto>> GetAllPaymentsAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            PaymentId,
            OrderId,
            PaymentReference,
            PaymentMethod,
            PaymentStatus,
            Amount,
            Currency,
            TransactionId,
            GatewayName,
            PaymentDate,
            FailureReason,
            RefundStatus
        FROM Payments;
        """;

    var result = await connection.QueryAsync<PaymentStatusDto>(sql);
    return result.ToList();
}

public async Task<IReadOnlyList<ShipmentStatusDto>> GetAllShipmentsAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            ShipmentId,
            OrderId,
            ShipmentReference,
            CarrierName,
            TrackingNumber,
            ShipmentStatus,
            ShippingMethod,
            ShippedDate,
            EstimatedDeliveryDate,
            DeliveredDate,
            DeliveryAttemptCount,
            LastLocation
        FROM Shipments;
        """;

    var result = await connection.QueryAsync<ShipmentStatusDto>(sql);
    return result.ToList();
}

public async Task<IReadOnlyList<ReturnStatusDto>> GetAllReturnsAsync()
{
    using var connection = _context.CreateConnection();

    const string sql = """
        SELECT
            ReturnId,
            OrderId,
            ReturnReference,
            ReturnReason,
            ReturnStatus,
            RefundAmount,
            RefundStatus,
            RequestedDate,
            ApprovedDate,
            RefundedDate,
            CustomerComments,
            InternalNotes
        FROM Returns;
        """;

    var result = await connection.QueryAsync<ReturnStatusDto>(sql);
    return result.ToList();
}
}