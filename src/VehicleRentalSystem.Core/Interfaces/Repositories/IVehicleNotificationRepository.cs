using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Repositories;

public interface IVehicleNotificationRepository
{
    Task<VehicleNotification?> GetByVehicleId(Guid vehicleId);
}
