using Microsoft.EntityFrameworkCore;
using ShippingPlatform.Domain.Entities;

namespace ShippingPlatform.Infrastructure.Persistence;

/// <summary>EF Core DbContext for the GoLIMS Shipping Platform's SQL Server database of record.</summary>
public class ShippingDbContext : DbContext
{
    public ShippingDbContext(DbContextOptions<ShippingDbContext> options) : base(options)
    {
    }

    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<ShipmentStatusHistory> ShipmentStatusHistories => Set<ShipmentStatusHistory>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<Carrier> Carriers => Set<Carrier>();
    public DbSet<CarrierService> CarrierServices => Set<CarrierService>();
    public DbSet<Kit> Kits => Set<Kit>();
    public DbSet<KitComponent> KitComponents => Set<KitComponent>();
    public DbSet<ChainOfCustodyEvent> ChainOfCustodyEvents => Set<ChainOfCustodyEvent>();
    public DbSet<Label> Labels => Set<Label>();
    public DbSet<LabelPrintEvent> LabelPrintEvents => Set<LabelPrintEvent>();
    public DbSet<RuleDefinition> RuleDefinitions => Set<RuleDefinition>();
    public DbSet<D365SyncRecord> D365SyncRecords => Set<D365SyncRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ShippingDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
