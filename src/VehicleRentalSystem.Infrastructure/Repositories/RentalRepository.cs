using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Infrastructure.Context;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class RentalRepository : Repository<Rental>, IRentalRepository
{
    public RentalRepository(DataContext dataContext, INotifier notifier) : base(dataContext, notifier)
    {
    }

    public async Task<IEnumerable<Rental>> GetByCourierId(Guid courierId)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(Rental)} by Courier ID {courierId}.");
            return await _dbSet.Where(r => r.CourierId == courierId).ToListAsync();
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(Rental)} by Courier ID {courierId}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<IEnumerable<Rental>> GetByMotorcycleId(Guid vehicleId)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(Rental)} by Motorcycle ID {vehicleId}.");
            return await _dbSet.Where(r => r.VehicleId == vehicleId).ToListAsync();
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(Rental)} by Motorcycle ID {vehicleId}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<IEnumerable<Rental>> GetActiveRentals()
    {
        try
        {
            _notifier.Handle($"Getting active {nameof(Rental)}.");
            return await _dbSet.Where(r => r.EndDate > DateTime.UtcNow).ToListAsync();
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting active {nameof(Rental)}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<decimal> CalculateRentalCost(Rental rental)
    {
        if (rental == null)
        {
            throw new ArgumentNullException(nameof(rental), "Rental cannot be null");
        }

        try
        {
            _notifier.Handle($"Calculating rental cost for rental with Motorcycle ID {rental.VehicleId} and Courier ID {rental.CourierId}.");

            if (!rental.EndDate.HasValue)
            {
                throw new InvalidOperationException("Rental EndDate must be specified to calculate the cost.");
            }

            var endDate = rental.EndDate.Value;
            var daysRented = (endDate - rental.StartDate).Days;
            var cost = daysRented * rental.DailyRate;

            if (endDate < rental.ExpectedEndDate)
            {
                var penaltyRate = rental.DailyRate * 0.20m;
                cost += penaltyRate * (rental.ExpectedEndDate - endDate).Days;
            }
            else if (endDate > rental.ExpectedEndDate)
            {
                var additionalDays = (endDate - rental.ExpectedEndDate).Days;
                cost += additionalDays * 50;
            }

            return cost;
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error calculating rental cost for rental with Motorcycle ID {rental.VehicleId} and Courier ID {rental.CourierId}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }
}
