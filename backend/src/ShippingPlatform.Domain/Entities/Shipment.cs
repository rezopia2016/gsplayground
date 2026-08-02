using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Domain.Entities;

/// <summary>
/// System-of-record shipment for the GoLIMS Shipping Platform. References D365 order/customer
/// data by external ID rather than duplicating it as a source of truth.
/// </summary>
public class Shipment
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>D365 SalesOrder/Data Entity key this shipment originated from, if any.</summary>
    public string? D365OrderReference { get; set; }

    public string ShipmentNumber { get; set; } = string.Empty;
    public ShipmentDirection Direction { get; set; }
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Created;
    public MaterialClassification MaterialClassification { get; set; } = MaterialClassification.Standard;

    public string OriginCountryCode { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;

    public Guid? CarrierId { get; set; }
    public Carrier? Carrier { get; set; }
    public string? CarrierServiceCode { get; set; }
    public string? CarrierTrackingNumber { get; set; }

    public Guid? ConsolidatedLoadId { get; set; }
    public Guid? KitId { get; set; }
    public Kit? Kit { get; set; }

    public decimal? FreightCost { get; set; }
    public string CurrencyCode { get; set; } = "USD";

    public bool RequiresTemperatureControl { get; set; }
    public bool IsVipHandling { get; set; }
    public string? CustomerType { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? DispatchedAtUtc { get; set; }
    public DateTimeOffset? DeliveredAtUtc { get; set; }

    public List<Package> Packages { get; set; } = new();
    public List<ShipmentStatusHistory> StatusHistory { get; set; } = new();
}

public class ShipmentStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShipmentId { get; set; }
    public ShipmentStatus Status { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? ChangedByUserId { get; set; }
    public string? Notes { get; set; }
}

public class Package
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShipmentId { get; set; }
    public string PackageNumber { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
    public string? LicensePlate { get; set; }
    public List<Label> Labels { get; set; } = new();
}
