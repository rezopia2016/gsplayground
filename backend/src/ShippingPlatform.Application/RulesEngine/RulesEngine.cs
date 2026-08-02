using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Entities;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Application.RulesEngine;

/// <summary>
/// Evaluates externally configured <see cref="RuleDefinition"/> records (E5) against a
/// <see cref="ShipmentContext"/> without requiring an application redeploy (BR-041/E5.S5).
///
/// Condition/action expressions use a small, deliberately simple grammar so rule authors
/// (compliance/ops admins) can configure rules through data rather than code:
///
///   Condition: comma-separated "key=value" clauses, all of which must match, e.g.
///     "MaterialClassification=BiologicalSample,DestinationCountryCode=DE"
///   Action: comma-separated action keywords, e.g.
///     "RequireHazardLabel,RequireComplianceDocumentation"
///
/// This is intentionally not a general-purpose expression language — replace with a proper
/// rules DSL/engine (e.g. a JSON-logic library) if rule complexity grows beyond simple
/// attribute matching.
/// </summary>
public class RulesEngine : IRulesEngine
{
    private readonly IRuleRepository _ruleRepository;

    public RulesEngine(IRuleRepository ruleRepository)
    {
        _ruleRepository = ruleRepository;
    }

    public async Task<RuleEvaluationResult> EvaluateAsync(ShipmentContext context, CancellationToken ct = default)
    {
        var result = new RuleEvaluationResult();
        var rules = await _ruleRepository.GetActiveRulesAsync(ct);

        foreach (var rule in rules.OrderByDescending(r => r.Priority))
        {
            if (!string.IsNullOrWhiteSpace(rule.ApplicableCountryCode)
                && !string.Equals(rule.ApplicableCountryCode, context.DestinationCountryCode, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!Matches(rule.ConditionExpression, context))
            {
                continue;
            }

            result.AppliedRuleNames.Add(rule.Name);
            ApplyActions(rule.ActionExpression, result);
        }

        return result;
    }

    private static bool Matches(string conditionExpression, ShipmentContext context)
    {
        if (string.IsNullOrWhiteSpace(conditionExpression))
        {
            return true;
        }

        var clauses = conditionExpression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var clause in clauses)
        {
            var parts = clause.Split('=', 2);
            if (parts.Length != 2)
            {
                continue;
            }

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            var actual = key switch
            {
                nameof(ShipmentContext.OriginCountryCode) => context.OriginCountryCode,
                nameof(ShipmentContext.DestinationCountryCode) => context.DestinationCountryCode,
                nameof(ShipmentContext.MaterialClassification) => context.MaterialClassification.ToString(),
                nameof(ShipmentContext.RequiresTemperatureControl) => context.RequiresTemperatureControl.ToString(),
                nameof(ShipmentContext.CustomerType) => context.CustomerType ?? string.Empty,
                nameof(ShipmentContext.IsVipHandling) => context.IsVipHandling.ToString(),
                _ => null
            };

            if (actual is null || !string.Equals(actual, value, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }

        return true;
    }

    private static void ApplyActions(string actionExpression, RuleEvaluationResult result)
    {
        var actions = actionExpression.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (var action in actions)
        {
            switch (action)
            {
                case "RequireHazardLabel":
                    result.RequiresHazardLabel = true;
                    break;
                case "RequireExpeditedService":
                    result.RequiresExpeditedService = true;
                    break;
                case "RequireComplianceDocumentation":
                    result.RequiresComplianceDocumentation = true;
                    break;
                default:
                    if (action.StartsWith("Block:", StringComparison.OrdinalIgnoreCase))
                    {
                        result.IsBlocked = true;
                        result.BlockedReason = action["Block:".Length..];
                    }
                    break;
            }
        }
    }
}
