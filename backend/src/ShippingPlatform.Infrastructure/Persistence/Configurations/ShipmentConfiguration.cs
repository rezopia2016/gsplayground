using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ShippingPlatform.Domain.Entities;

namespace ShippingPlatform.Infrastructure.Persistence.Configurations;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.ToTable("Shipments");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.ShipmentNumber).HasMaxLength(30).IsRequired();
        builder.HasIndex(s => s.ShipmentNumber).IsUnique();
        builder.Property(s => s.OriginCountryCode).HasMaxLength(2).IsRequired();
        builder.Property(s => s.DestinationCountryCode).HasMaxLength(2).IsRequired();
        builder.Property(s => s.FreightCost).HasColumnType("decimal(18,2)");
        builder.Property(s => s.CurrencyCode).HasMaxLength(3);

        builder.HasOne(s => s.Carrier)
            .WithMany()
            .HasForeignKey(s => s.CarrierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Kit)
            .WithMany()
            .HasForeignKey(s => s.KitId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Packages)
            .WithOne()
            .HasForeignKey(p => p.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.StatusHistory)
            .WithOne()
            .HasForeignKey(h => h.ShipmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PackageConfiguration : IEntityTypeConfiguration<Package>
{
    public void Configure(EntityTypeBuilder<Package> builder)
    {
        builder.ToTable("Packages");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.PackageNumber).HasMaxLength(30).IsRequired();
        builder.Property(p => p.WeightKg).HasColumnType("decimal(10,3)");

        builder.HasMany(p => p.Labels)
            .WithOne()
            .HasForeignKey(l => l.PackageId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
