using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.RulesEngine;

/// <summary>Read-only context a rule condition is evaluated against (E5.S1).</summary>
public record ShipmentContext(
    string OriginCountryCode,
    string DestinationCountryCode,
    MaterialClassification MaterialClassification,
    bool RequiresTemperatureControl,
    string? CustomerType,
    bool IsVipHandling,
    DateTimeOffset? RequestedPickupUtc);

/// <summary>Outcome of rule evaluation: side effects the caller must apply to the shipment/kit.</summary>
public class RuleEvaluationResult
{
    public bool RequiresHazardLabel { get; set; }
    public bool RequiresExpeditedService { get; set; }
    public bool RequiresComplianceDocumentation { get; set; }
    public bool IsBlocked { get; set; }
    public string? BlockedReason { get; set; }
    public List<string> AppliedRuleNames { get; } = new();
}
