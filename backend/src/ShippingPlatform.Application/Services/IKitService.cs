using ShippingPlatform.Application.DTOs;

namespace ShippingPlatform.Application.Services;

public interface IKitService
{
    Task<KitResponse> CreateKitAsync(CreateKitRequest request, CancellationToken ct = default);
    Task<KitResponse?> GetKitAsync(Guid id, CancellationToken ct = default);
    Task<KitResponse> FulfillComponentAsync(Guid kitId, string componentCode, CancellationToken ct = default);
    Task<KitResponse> DispatchKitAsync(Guid kitId, string? actorUserId, string? location, CancellationToken ct = default);
    Task RecordCustodyEventAsync(Guid kitId, RecordCustodyEventRequest request, CancellationToken ct = default);
}
