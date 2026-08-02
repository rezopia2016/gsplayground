using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Domain.Entities;

/// <summary>
/// Outbox-pattern record for D365 integration (BR-063/E7.S5). Every inbound/outbound sync
/// operation is recorded here first and processed asynchronously, so shipping operations are
/// never blocked by D365 latency or downtime.
/// </summary>
public class D365SyncRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public D365SyncDirection Direction { get; set; }
    public D365SyncEntityType EntityType { get; set; }
    public D365SyncStatus Status { get; set; } = D365SyncStatus.Pending;

    public Guid? ShipmentId { get; set; }
    public Guid? KitId { get; set; }

    /// <summary>Serialized payload sent to/received from D365 (JSON).</summary>
    public string Payload { get; set; } = string.Empty;

    public int AttemptCount { get; set; }
    public string? LastError { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastAttemptAtUtc { get; set; }
    public DateTimeOffset? CompletedAtUtc { get; set; }
}
