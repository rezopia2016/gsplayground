using ShippingPlatform.Application.DTOs;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Entities;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.Services;

/// <summary>
/// Kit &amp; collection logistics service (E4) — the domain D365 rates lowest (Kit Logistics
/// ⭐☆☆☆☆). Enforces packaging completeness validation (BR-032) and records an immutable,
/// append-only chain of custody (BR-033).
/// </summary>
public class KitService : IKitService
{
    private readonly IKitRepository _kits;

    public KitService(IKitRepository kits)
    {
        _kits = kits;
    }

    public async Task<KitResponse> CreateKitAsync(CreateKitRequest request, CancellationToken ct = default)
    {
        var kit = new Kit
        {
            KitTypeCode = request.KitTypeCode,
            D365OrderReference = request.D365OrderReference,
            KitNumber = $"KIT-{DateTimeOffset.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpperInvariant()}"
        };

        foreach (var c in request.Components)
        {
            kit.Components.Add(new KitComponent
            {
                KitId = kit.Id,
                ComponentCode = c.ComponentCode,
                ComponentName = c.ComponentName,
                Quantity = c.Quantity,
                IsRequired = c.IsRequired,
                ConsumableSku = c.ConsumableSku
            });
        }

        kit.CustodyEvents.Add(new ChainOfCustodyEvent { KitId = kit.Id, Action = CustodyAction.Created });

        await _kits.AddAsync(kit, ct);
        return ToResponse(kit);
    }

    public async Task<KitResponse?> GetKitAsync(Guid id, CancellationToken ct = default)
    {
        var kit = await _kits.GetByIdAsync(id, ct);
        return kit is null ? null : ToResponse(kit);
    }

    public async Task<KitResponse> FulfillComponentAsync(Guid kitId, string componentCode, CancellationToken ct = default)
    {
        var kit = await _kits.GetByIdAsync(kitId, ct) ?? throw new KeyNotFoundException($"Kit {kitId} not found.");
        var component = kit.Components.FirstOrDefault(c => c.ComponentCode == componentCode)
            ?? throw new KeyNotFoundException($"Component {componentCode} not found on kit {kitId}.");

        component.IsFulfilled = true;
        kit.Status = kit.IsComplete() ? KitStatus.ReadyForDispatch : KitStatus.Assembling;

        await _kits.UpdateAsync(kit, ct);
        return ToResponse(kit);
    }

    public async Task<KitResponse> DispatchKitAsync(Guid kitId, string? actorUserId, string? location, CancellationToken ct = default)
    {
        var kit = await _kits.GetByIdAsync(kitId, ct) ?? throw new KeyNotFoundException($"Kit {kitId} not found.");

        if (!kit.IsComplete())
        {
            kit.Status = KitStatus.Incomplete;
            await _kits.UpdateAsync(kit, ct);
            var missing = kit.Components.Where(c => c.IsRequired && !c.IsFulfilled).Select(c => c.ComponentCode);
            throw new InvalidOperationException($"Kit {kit.KitNumber} is missing required component(s): {string.Join(", ", missing)}.");
        }

        kit.Status = KitStatus.Dispatched;
        kit.CustodyEvents.Add(new ChainOfCustodyEvent
        {
            KitId = kit.Id,
            Action = CustodyAction.Dispatched,
            ActorUserId = actorUserId,
            Location = location
        });

        await _kits.UpdateAsync(kit, ct);
        return ToResponse(kit);
    }

    public async Task RecordCustodyEventAsync(Guid kitId, RecordCustodyEventRequest request, CancellationToken ct = default)
    {
        var kit = await _kits.GetByIdAsync(kitId, ct) ?? throw new KeyNotFoundException($"Kit {kitId} not found.");

        // Custody events are append-only (TC-E4-07) — never mutate or remove prior events.
        kit.CustodyEvents.Add(new ChainOfCustodyEvent
        {
            KitId = kit.Id,
            ShipmentId = request.ShipmentId,
            Action = request.Action,
            Location = request.Location,
            ActorUserId = request.ActorUserId,
            Notes = request.Notes
        });

        kit.Status = request.Action switch
        {
            CustodyAction.PickedUp => KitStatus.InTransit,
            CustodyAction.Received => KitStatus.Received,
            CustodyAction.Returned => KitStatus.Returned,
            _ => kit.Status
        };

        await _kits.UpdateAsync(kit, ct);
    }

    private static KitResponse ToResponse(Kit kit) => new(
        kit.Id,
        kit.KitNumber,
        kit.KitTypeCode,
        kit.Status,
        kit.IsComplete(),
        kit.Components.Select(c => new KitComponentResponse(c.ComponentCode, c.ComponentName, c.Quantity, c.IsRequired, c.IsFulfilled)).ToList());
}
