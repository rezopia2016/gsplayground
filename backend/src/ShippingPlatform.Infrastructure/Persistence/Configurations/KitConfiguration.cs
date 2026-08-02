using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingPlatform.Domain.Entities;

namespace ShippingPlatform.Infrastructure.Persistence.Configurations;

public class KitConfiguration : IEntityTypeConfiguration<Kit>
{
    public void Configure(EntityTypeBuilder<Kit> builder)
    {
        builder.ToTable("Kits");
        builder.HasKey(k => k.Id);
        builder.Property(k => k.KitNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(k => k.KitNumber).IsUnique();
        builder.Ignore(k => k.IsComplete);

        builder.HasMany(k => k.Components)
            .WithOne()
            .HasForeignKey(c => c.KitId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(k => k.CustodyEvents)
            .WithOne()
            .HasForeignKey(e => e.KitId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>Custody events are append-only in the domain; the database enforces no update/delete via a trigger (see database/schema/003_custody_immutability.sql).</summary>
public class ChainOfCustodyEventConfiguration : IEntityTypeConfiguration<ChainOfCustodyEvent>
{
    public void Configure(EntityTypeBuilder<ChainOfCustodyEvent> builder)
    {
        builder.ToTable("ChainOfCustodyEvents");
        builder.HasKey(e => e.Id);
    }
}
