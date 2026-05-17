namespace CustomerSupport.Api.Models;

public class Shipment
{
    public int ShipmentId { get; set; }

    public int OrderId { get; set; }

    public string? ShipmentReference { get; set; }

    public string? CarrierName { get; set; }

    public string? TrackingNumber { get; set; }

    public string? ShipmentStatus { get; set; }

    public string? ShippingMethod { get; set; }

    public DateTime? ShippedDate { get; set; }

    public DateTime? EstimatedDeliveryDate { get; set; }

    public DateTime? DeliveredDate { get; set; }

    public int DeliveryAttemptCount { get; set; }

    public string? LastLocation { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }
}