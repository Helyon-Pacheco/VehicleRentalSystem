using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Infrastructure.Mappings;

public class VehicleMapping : EntityBaseMapping<Vehicle>
{
    public override void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        base.Configure(builder);

        builder.ToTable("Vehicles");

        builder.Property(m => m.Year)
            .IsRequired();

        builder.Property(m => m.Model)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(m => m.Plate)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(m => m.VehicleType)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(m => m.Plate)
            .IsUnique();

        builder.HasOne(m => m.VehicleNotification)
            .WithOne(n => n.Vehicle)
            .HasForeignKey<VehicleNotification>(n => n.VehicleId)
            .IsRequired(false);
    }
}
