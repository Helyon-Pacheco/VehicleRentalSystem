using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Infrastructure.Mappings;

public class CourierMapping : EntityBaseMapping<Courier>
{
    public override void Configure(EntityTypeBuilder<Courier> builder)
    {
        base.Configure(builder);

        builder.ToTable("Couriers");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.Cnpj)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.Cnpj)
            .IsUnique();

        builder.Property(c => c.BirthDate)
            .IsRequired();

        builder.Property(c => c.CnhNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(c => c.CnhNumber)
            .IsUnique();

        builder.Property(c => c.CnhType)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(c => c.CnhImage)
            .HasMaxLength(255)
            .IsRequired(false);
    }
}
