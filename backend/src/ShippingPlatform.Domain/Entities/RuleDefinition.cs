namespace ShippingPlatform.Domain.Entities;

/// <summary>
/// Externalized business rule (BR-040/041). Stored as data so operations can change shipping
/// behavior (VIP handling, biological-material handling, pickup-window routing, regional
/// compliance) without an application release. Evaluated by the Rules Engine against a
/// ShipmentContext at shipment-creation and dispatch time.
/// </summary>
public class RuleDefinition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g. "CustomerHandling", "Compliance", "Routing"
    public int Priority { get; set; }
    public bool IsActive { get; set; } = true;

    /// <summary>Condition expression evaluated against ShipmentContext (e.g. a simple DSL or JSON logic tree).</summary>
    public string ConditionExpression { get; set; } = string.Empty;

    /// <summary>Actions to apply when the condition matches (e.g. "RequireHazardLabel", "SetExpeditedService").</summary>
    public string ActionExpression { get; set; } = string.Empty;

    public string? ApplicableCountryCode { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? UpdatedAtUtc { get; set; }
}
