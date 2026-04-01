using Microsoft.EntityFrameworkCore.Storage;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Interfaces.UoW;
using VehicleRentalSystem.Infrastructure.Context;

namespace VehicleRentalSystem.Infrastructure.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _dataContext;
    private IDbContextTransaction? _transaction;

    public IVehicleRepository Vehicles {  get; }
    public ICourierRepository Couriers { get; }
    public IRentalRepository Rentals { get; }
    public IVehicleNotificationRepository VehicleNotifications { get; }

    public UnitOfWork(DataContext dataContext,
        IVehicleRepository vehicleRepository,
        ICourierRepository courierRepository,
        IRentalRepository rentalRepository,
        IVehicleNotificationRepository vehicleNotifications)
    {
        _dataContext = dataContext;
        Vehicles = vehicleRepository;
        Couriers = courierRepository;
        Rentals = rentalRepository;
        VehicleNotifications = vehicleNotifications;
    }

    public async Task<int> SaveAsync()
    {
        return await _dataContext.SaveChangesAsync();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        if (_transaction == null)
        {
            _transaction = await _dataContext.Database.BeginTransactionAsync();
        }
        return _transaction;
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _dataContext.Dispose();
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }
        }
    }
}
