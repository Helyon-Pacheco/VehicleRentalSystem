using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Models.Enums;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Infrastructure.Context;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(DataContext dataContext, INotifier notifier) : base(dataContext, notifier)
    {
    }

    public async Task<Vehicle?> GetByPlate(string plate)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(Vehicle)} by Plate {plate}.");
            return await _dbSet.FirstOrDefaultAsync(m => m.Plate == plate);
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(Vehicle)} by Plate {plate}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<Vehicle?> GetByType(VehicleType vehicleType)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(Vehicle)} by Type {vehicleType}.");
            return await _dbSet.FirstOrDefaultAsync(m => m.VehicleType == vehicleType);
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(Vehicle)} by Type {vehicleType}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<IEnumerable<Vehicle>> GetAllByYear(int year)
    {
        try
        {
            _notifier.Handle($"Getting all {nameof(Vehicle)} by Year {year}.");
            return await _dbSet.Where(m => m.Year == year).ToListAsync();
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting all {nameof(Vehicle)} by Year {year}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }
}
