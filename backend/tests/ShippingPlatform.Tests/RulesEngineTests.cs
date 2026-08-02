using Moq;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Application.RulesEngine;
using ShippingPlatform.Domain.Entities;
using ShippingPlatform.Domain.Enums;
using Xunit;
using RulesEngineImpl = ShippingPlatform.Application.RulesEngine.RulesEngine;

namespace ShippingPlatform.Tests;

/// <summary>Covers TC-E5-01/02/06 (rule matching, precedence, biological-material enforcement).</summary>
public class RulesEngineTests
{
    [Fact]
    public async Task EvaluateAsync_AppliesHazardLabel_ForBiologicalMaterial()
    {
        var ruleRepository = new Mock<IRuleRepository>();
        ruleRepository.Setup(r => r.GetActiveRulesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RuleDefinition>
            {
                new()
                {
                    Name = "Biological material requires hazard label",
                    Priority = 10,
                    ConditionExpression = "MaterialClassification=BiologicalSample",
                    ActionExpression = "RequireHazardLabel,RequireComplianceDocumentation"
                }
            });

        var engine = new RulesEngineImpl(ruleRepository.Object);
        var context = new ShipmentContext("US", "DE", MaterialClassification.BiologicalSample, true, "Standard", false, null);

        var result = await engine.EvaluateAsync(context);

        Assert.True(result.RequiresHazardLabel);
        Assert.True(result.RequiresComplianceDocumentation);
        Assert.Contains("Biological material requires hazard label", result.AppliedRuleNames);
    }

    [Fact]
    public async Task EvaluateAsync_HigherPriorityRuleAppliesFirst_BothRulesRecorded()
    {
        var ruleRepository = new Mock<IRuleRepository>();
        ruleRepository.Setup(r => r.GetActiveRulesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RuleDefinition>
            {
                new() { Name = "Low priority VIP rule", Priority = 1, ConditionExpression = "IsVipHandling=True", ActionExpression = "RequireExpeditedService" },
                new() { Name = "High priority compliance block", Priority = 100, ConditionExpression = "DestinationCountryCode=JP", ActionExpression = "Block:Missing Japan compliance documentation" }
            });

        var engine = new RulesEngineImpl(ruleRepository.Object);
        var context = new ShipmentContext("US", "JP", MaterialClassification.Standard, false, "Standard", true, null);

        var result = await engine.EvaluateAsync(context);

        Assert.True(result.IsBlocked);
        Assert.Equal("Missing Japan compliance documentation", result.BlockedReason);
        Assert.Equal(2, result.AppliedRuleNames.Count);
    }

    [Fact]
    public async Task EvaluateAsync_CountryScopedRule_DoesNotApplyOutsideCountry()
    {
        var ruleRepository = new Mock<IRuleRepository>();
        ruleRepository.Setup(r => r.GetActiveRulesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<RuleDefinition>
            {
                new() { Name = "EU-only rule", Priority = 5, ApplicableCountryCode = "DE", ConditionExpression = "", ActionExpression = "RequireComplianceDocumentation" }
            });

        var engine = new RulesEngineImpl(ruleRepository.Object);
        var context = new ShipmentContext("US", "JP", MaterialClassification.Standard, false, null, false, null);

        var result = await engine.EvaluateAsync(context);

        Assert.False(result.RequiresComplianceDocumentation);
        Assert.Empty(result.AppliedRuleNames);
    }
}
