using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Domain.Entities;

/// <summary>
/// Collection/DNA kit domain model — the capability D365 rates lowest (Kit Logistics, custom
/// only). Models multi-component assembly, partial fulfillment, and full chain of custody.
/// </summary>
public class Kit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string KitNumber { get; set; } = string.Empty;
    public string KitTypeCode { get; set; } = string.Empty;
    public KitStatus Status { get; set; } = KitStatus.Created;
    public string? D365OrderReference { get; set; }

    public List<KitComponent> Components { get; set; } = new();
    public List<ChainOfCustodyEvent> CustodyEvents { get; set; } = new();

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;

    public bool IsComplete() => Components.All(c => c.IsFulfilled || !c.IsRequired);
}

public class KitComponent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KitId { get; set; }
    public string ComponentCode { get; set; } = string.Empty;
    public string ComponentName { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
    public bool IsRequired { get; set; } = true;
    public bool IsFulfilled { get; set; }
    public string? ConsumableSku { get; set; }
}

/// <summary>
/// Immutable, append-only chain-of-custody event for biological samples/kits. Never updated or
/// deleted — corrections are recorded as new events referencing the original.
/// </summary>
public class ChainOfCustodyEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid KitId { get; set; }
    public Guid? ShipmentId { get; set; }
    public CustodyAction Action { get; set; }
    public string? Location { get; set; }
    public string? ActorUserId { get; set; }
    public DateTimeOffset OccurredAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public string? Notes { get; set; }
}
