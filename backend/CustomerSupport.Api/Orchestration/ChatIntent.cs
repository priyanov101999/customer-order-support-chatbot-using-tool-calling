namespace CustomerSupport.Api.Orchestration;

public enum ChatIntent
{
    Unknown,
    OrderSummary,
    PaymentStatus,
    ShipmentStatus,
    ReturnStatus,
    CustomerOverview,
    CustomerOrderHistory,
    PendingRefunds
}