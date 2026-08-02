using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.DTOs;

public record CreateShipmentRequest(
    string? D365OrderReference,
    ShipmentDirection Direction,
    string OriginCountryCode,
    string DestinationCountryCode,
    string OriginAddress,
    string DestinationAddress,
    MaterialClassification MaterialClassification,
    bool RequiresTemperatureControl,
    string? CustomerType,
    bool IsVipHandling,
    Guid? KitId);

public record ShipmentResponse(
    Guid Id,
    string ShipmentNumber,
    ShipmentStatus Status,
    ShipmentDirection Direction,
    string OriginCountryCode,
    string DestinationCountryCode,
    string? CarrierCode,
    string? CarrierServiceCode,
    string? CarrierTrackingNumber,
    decimal? FreightCost,
    string CurrencyCode,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? DispatchedAtUtc,
    DateTimeOffset? DeliveredAtUtc);

public record RateQuoteRequest(
    string OriginCountryCode,
    string DestinationCountryCode,
    decimal TotalWeightKg,
    MaterialClassification MaterialClassification,
    bool RequiresTemperatureControl);

public record RateQuoteResponse(
    string CarrierCode,
    string ServiceCode,
    string ServiceName,
    decimal Amount,
    string CurrencyCode,
    int EstimatedTransitDays);
