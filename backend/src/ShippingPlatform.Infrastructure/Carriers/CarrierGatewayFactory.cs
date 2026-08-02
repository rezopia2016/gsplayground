using ShippingPlatform.Application.Interfaces;

namespace ShippingPlatform.Infrastructure.Carriers;

/// <summary>
/// Resolves the correct ICarrierGateway by adapter key. New carriers are onboarded by
/// registering another named implementation in DependencyInjection.cs and a matching
/// Carrier.AdapterKey row in the database — no changes to ShipmentService or the API surface
/// (BR-011/E2.S5).
/// </summary>
public class CarrierGatewayFactory : ICarrierGatewayFactory
{
    private readonly IEnumerable<ICarrierGateway> _gateways;

    public CarrierGatewayFactory(IEnumerable<ICarrierGateway> gateways)
    {
        _gateways = gateways;
    }

    public ICarrierGateway Resolve(string adapterKey)
    {
        var gateway = _gateways.FirstOrDefault(g => string.Equals(g.AdapterKey, adapterKey, StringComparison.OrdinalIgnoreCase));
        return gateway ?? throw new NotSupportedException($"No carrier gateway registered for adapter key '{adapterKey}'.");
    }
}
