namespace ShippingPlatform.Application.RulesEngine;

public interface IRulesEngine
{
    Task<RuleEvaluationResult> EvaluateAsync(ShipmentContext context, CancellationToken ct = default);
}
