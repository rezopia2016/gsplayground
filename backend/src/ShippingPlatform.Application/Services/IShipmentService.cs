using ShippingPlatform.Application.DTOs;

namespace ShippingPlatform.Application.Services;

public interface IShipmentService
{
    Task<ShipmentResponse> CreateShipmentAsync(CreateShipmentRequest request, CancellationToken ct = default);
    Task<ShipmentResponse?> GetShipmentAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<ShipmentResponse>> ListShipmentsAsync(CancellationToken ct = default);
    Task<ShipmentResponse> DispatchAsync(Guid id, string carrierCode, string serviceCode, CancellationToken ct = default);
    Task<ShipmentResponse> CancelAsync(Guid id, string? reason, CancellationToken ct = default);
    Task<IReadOnlyList<RateQuoteResponse>> GetRatesAsync(RateQuoteRequest request, CancellationToken ct = default);
}
