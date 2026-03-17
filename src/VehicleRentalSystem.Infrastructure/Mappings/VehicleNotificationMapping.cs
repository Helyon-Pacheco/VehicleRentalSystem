using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Infrastructure.Mappings;

public class VehicleNotificationMapping : EntityBaseMapping<VehicleNotification>
{
    public override void Configure(EntityTypeBuilder<VehicleNotification> builder)
    {
        base.Configure(builder);

        builder.ToTable("VehicleNotifications");

        builder.Property(n => n.Message)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasOne(n => n.Vehicle)
            .WithOne(m => m.VehicleNotification)
            .HasForeignKey<VehicleNotification>(n => n.VehicleId);
    }
}
