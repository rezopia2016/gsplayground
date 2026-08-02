using ShippingPlatform.Domain.Entities;

namespace ShippingPlatform.Application.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Shipment>> ListAsync(CancellationToken ct = default);
    Task AddAsync(Shipment shipment, CancellationToken ct = default);
    Task UpdateAsync(Shipment shipment, CancellationToken ct = default);
}

public interface IKitRepository
{
    Task<Kit?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Kit kit, CancellationToken ct = default);
    Task UpdateAsync(Kit kit, CancellationToken ct = default);
}

public interface ICarrierRepository
{
    Task<IReadOnlyList<Carrier>> GetActiveCarriersAsync(CancellationToken ct = default);
    Task<Carrier?> GetByCodeAsync(string code, CancellationToken ct = default);
}

public interface ILabelRepository
{
    Task<Label?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Label label, CancellationToken ct = default);
    Task UpdateAsync(Label label, CancellationToken ct = default);
}

public interface IRuleRepository
{
    Task<IReadOnlyList<Domain.Entities.RuleDefinition>> GetActiveRulesAsync(CancellationToken ct = default);
}

public interface ID365SyncRepository
{
    Task EnqueueAsync(Domain.Entities.D365SyncRecord record, CancellationToken ct = default);
    Task<IReadOnlyList<Domain.Entities.D365SyncRecord>> GetPendingAsync(int maxAttempts, CancellationToken ct = default);
    Task UpdateAsync(Domain.Entities.D365SyncRecord record, CancellationToken ct = default);
}
