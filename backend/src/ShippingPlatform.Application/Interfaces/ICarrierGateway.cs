using ShippingPlatform.Application.DTOs;

namespace ShippingPlatform.Application.Interfaces;

/// <summary>
/// Common abstraction every carrier adapter (DHL, UPS, Nitsu, future carriers) implements.
/// This is the interface that lets a new regional carrier be onboarded as a plug-in
/// (BR-011–013 / E2) instead of a shipment-engine or ERP change.
/// </summary>
public interface ICarrierGateway
{
    /// <summary>Unique key matching Carrier.AdapterKey, e.g. "dhl", "ups", "nitsu".</summary>
    string AdapterKey { get; }

    Task<IReadOnlyList<RateQuoteResponse>> GetRatesAsync(RateQuoteRequest request, CancellationToken ct = default);

    Task<CarrierLabelResult> CreateLabelAsync(CarrierLabelRequest request, CancellationToken ct = default);

    Task<CarrierTrackingResult> GetTrackingAsync(string trackingNumber, CancellationToken ct = default);

    Task VoidShipmentAsync(string trackingNumber, CancellationToken ct = default);
}

/// <summary>Resolves the correct ICarrierGateway implementation for a given carrier/adapter key.</summary>
public interface ICarrierGatewayFactory
{
    ICarrierGateway Resolve(string adapterKey);
}

public record CarrierLabelRequest(
    string ServiceCode,
    string OriginAddress,
    string DestinationAddress,
    decimal WeightKg,
    bool RequiresTemperatureControl,
    bool ContainsBiologicalMaterial);

public record CarrierLabelResult(string TrackingNumber, byte[] LabelZplOrPdf, string Format);

public record CarrierTrackingResult(string TrackingNumber, string StatusDescription, DateTimeOffset? EstimatedDelivery);
