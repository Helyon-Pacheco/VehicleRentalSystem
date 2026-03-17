using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Infrastructure.Mappings;

public abstract class EntityBaseMapping<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.CreatedByUser)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedAt)
            .IsRequired(false);

        builder.Property(e => e.UpdatedByUser)
            .IsRequired(false)
            .HasMaxLength(100);

        builder.Property(e => e.IsDeleted)
            .IsRequired();
    }
}
