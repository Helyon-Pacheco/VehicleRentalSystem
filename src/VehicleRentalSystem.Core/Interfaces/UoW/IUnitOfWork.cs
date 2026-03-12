using Microsoft.EntityFrameworkCore.Storage;
using VehicleRentalSystem.Core.Interfaces.Repositories;

namespace VehicleRentalSystem.Core.Interfaces.UoW;

public interface IUnitOfWork : IDisposable
{
    IVehicleRepository Vehicles { get; }
    ICourierRepository Couriers { get; }
    IRentalRepository Rentals { get; }
    IVehicleNotificationRepository VehicleNotifications { get; }
    Task<int> SaveAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
