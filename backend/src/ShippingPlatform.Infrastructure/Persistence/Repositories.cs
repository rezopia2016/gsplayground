using Microsoft.EntityFrameworkCore;
using ShippingPlatform.Application.Interfaces;
using ShippingPlatform.Domain.Entities;
using ShippingPlatform.Domain.Enums;

namespace ShippingPlatform.Infrastructure.Persistence;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ShippingDbContext _db;
    public ShipmentRepository(ShippingDbContext db) => _db = db;

    public async Task<Shipment?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Shipments
            .Include(s => s.Packages).ThenInclude(p => p.Labels)
            .Include(s => s.StatusHistory)
            .Include(s => s.Carrier)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public async Task<IReadOnlyList<Shipment>> ListAsync(CancellationToken ct = default) =>
        await _db.Shipments.Include(s => s.Carrier).OrderByDescending(s => s.CreatedAtUtc).ToListAsync(ct);

    public async Task AddAsync(Shipment shipment, CancellationToken ct = default)
    {
        _db.Shipments.Add(shipment);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Shipment shipment, CancellationToken ct = default)
    {
        _db.Shipments.Update(shipment);
        await _db.SaveChangesAsync(ct);
    }
}

public class KitRepository : IKitRepository
{
    private readonly ShippingDbContext _db;
    public KitRepository(ShippingDbContext db) => _db = db;

    public async Task<Kit?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Kits
            .Include(k => k.Components)
            .Include(k => k.CustodyEvents)
            .FirstOrDefaultAsync(k => k.Id == id, ct);

    public async Task AddAsync(Kit kit, CancellationToken ct = default)
    {
        _db.Kits.Add(kit);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Kit kit, CancellationToken ct = default)
    {
        _db.Kits.Update(kit);
        await _db.SaveChangesAsync(ct);
    }
}

public class CarrierRepository : ICarrierRepository
{
    private readonly ShippingDbContext _db;
    public CarrierRepository(ShippingDbContext db) => _db = db;

    public async Task<IReadOnlyList<Carrier>> GetActiveCarriersAsync(CancellationToken ct = default) =>
        await _db.Carriers.Where(c => c.IsActive).Include(c => c.Services).ToListAsync(ct);

    public async Task<Carrier?> GetByCodeAsync(string code, CancellationToken ct = default) =>
        await _db.Carriers.Include(c => c.Services).FirstOrDefaultAsync(c => c.Code == code && c.IsActive, ct);
}

public class LabelRepository : ILabelRepository
{
    private readonly ShippingDbContext _db;
    public LabelRepository(ShippingDbContext db) => _db = db;

    public async Task<Label?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _db.Labels.Include(l => l.PrintEvents).FirstOrDefaultAsync(l => l.Id == id, ct);

    public async Task AddAsync(Label label, CancellationToken ct = default)
    {
        _db.Labels.Add(label);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Label label, CancellationToken ct = default)
    {
        _db.Labels.Update(label);
        await _db.SaveChangesAsync(ct);
    }
}

public class RuleRepository : IRuleRepository
{
    private readonly ShippingDbContext _db;
    public RuleRepository(ShippingDbContext db) => _db = db;

    public async Task<IReadOnlyList<RuleDefinition>> GetActiveRulesAsync(CancellationToken ct = default) =>
        await _db.RuleDefinitions.Where(r => r.IsActive).ToListAsync(ct);
}

public class D365SyncRepository : ID365SyncRepository
{
    private readonly ShippingDbContext _db;
    public D365SyncRepository(ShippingDbContext db) => _db = db;

    public async Task EnqueueAsync(D365SyncRecord record, CancellationToken ct = default)
    {
        _db.D365SyncRecords.Add(record);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<D365SyncRecord>> GetPendingAsync(int maxAttempts, CancellationToken ct = default) =>
        await _db.D365SyncRecords
            .Where(r => (r.Status == D365SyncStatus.Pending || r.Status == D365SyncStatus.Retrying) && r.AttemptCount < maxAttempts)
            .OrderBy(r => r.CreatedAtUtc)
            .ToListAsync(ct);

    public async Task UpdateAsync(D365SyncRecord record, CancellationToken ct = default)
    {
        _db.D365SyncRecords.Update(record);
        await _db.SaveChangesAsync(ct);
    }
}
