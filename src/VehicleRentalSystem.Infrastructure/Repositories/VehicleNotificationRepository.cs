using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Infrastructure.Context;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class VehicleNotificationRepository : Repository<VehicleNotification>, IVehicleNotificationRepository
{
    public VehicleNotificationRepository(DataContext dataContext, INotifier notifier) : base(dataContext, notifier)
    {
    }

    public async Task<VehicleNotification> GetByVehicleId(Guid vehicleId)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(VehicleNotification)} by Motorcycle ID {vehicleId}.");
            return await _dbSet.FirstOrDefaultAsync(mn => mn.VehicleId == vehicleId);
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(VehicleNotification)} by Motorcycle ID {vehicleId}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }
}
