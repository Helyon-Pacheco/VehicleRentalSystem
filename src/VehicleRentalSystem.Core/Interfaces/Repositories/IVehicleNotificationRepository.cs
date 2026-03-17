using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Repositories;

public interface IVehicleNotificationRepository : IRepository<VehicleNotification>
{
    Task<VehicleNotification?> GetByVehicleId(Guid vehicleId);
}
