using CustomerSupport.Api.DTOs;

namespace CustomerSupport.Api.Repositories;

public interface ICustomerSupportRepository
{
    Task<CustomerDto?> GetCustomerByIdAsync(int customerId);
    Task<IReadOnlyList<AddressDto>> GetCustomerAddressesAsync(int customerId);

    Task<IReadOnlyList<ProductDto>> SearchProductsAsync(string searchText);

    Task<OrderSummaryDto?> GetOrderSummaryAsync(int orderId);
    Task<IReadOnlyList<CustomerOrderDto>> GetCustomerOrderHistoryAsync(int customerId);

    Task<PaymentStatusDto?> GetPaymentStatusAsync(int orderId);
    Task<ShipmentStatusDto?> GetShipmentStatusAsync(int orderId);
    Task<ReturnStatusDto?> GetReturnStatusAsync(int orderId);

    Task<CustomerOverviewDto?> GetCustomerOverviewAsync(int customerId);
    Task<IReadOnlyList<PendingRefundDto>> GetPendingRefundsAsync();
    Task<IReadOnlyList<CustomerDto>> GetAllCustomersAsync();
Task<IReadOnlyList<AddressDto>> GetAllAddressesAsync();
Task<IReadOnlyList<ProductDto>> GetAllProductsAsync();
Task<IReadOnlyList<OrderSummaryDto>> GetAllOrdersAsync();
Task<IReadOnlyList<PaymentStatusDto>> GetAllPaymentsAsync();
Task<IReadOnlyList<ShipmentStatusDto>> GetAllShipmentsAsync();
Task<IReadOnlyList<ReturnStatusDto>> GetAllReturnsAsync();
}