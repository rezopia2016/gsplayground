namespace ShippingPlatform.Domain.Entities;

/// <summary>
/// Carrier master data for the abstraction layer. AdapterKey selects the ICarrierGateway
/// implementation (e.g. "dhl", "ups", "nitsu") registered at startup, so onboarding a new
/// carrier is a configuration + plug-in exercise, not a core code change.
/// </summary>
public class Carrier
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AdapterKey { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool SupportsEdi { get; set; }
    public int Priority { get; set; }
    public List<CarrierService> Services { get; set; } = new();
}

public class CarrierService
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CarrierId { get; set; }
    public string ServiceCode { get; set; } = string.Empty;
    public string ServiceName { get; set; } = string.Empty;
    public bool SupportsTemperatureControl { get; set; }
    public bool SupportsBiologicalMaterial { get; set; }
}
