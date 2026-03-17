using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Infrastructure.Mappings;

namespace VehicleRentalSystem.Infrastructure.Context;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Courier> Couriers { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<VehicleNotification> VehicleNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new VehicleMapping());
        modelBuilder.ApplyConfiguration(new CourierMapping());
        modelBuilder.ApplyConfiguration(new RentalMapping());
        modelBuilder.ApplyConfiguration(new VehicleNotificationMapping());
    }
}
