using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.Core.Interfaces.Services;

public interface IVehicleService
{
    Task<Vehicle?> GetById(Guid id);
    Task<IEnumerable<Vehicle>> GetAll();
    Task<PaginatedResponse<Vehicle>> GetAllPaged(int page, int pageSize);
    Task<Vehicle?> GetByPlate(string plate);
    Task<Vehicle?> GetByType(VehicleType vehicleType);
    Task<IEnumerable<Vehicle>> GetAllByYear(int year);
    Task<VehicleNotification> GetVehicleNotification(Guid id);
    Task<bool> Add(Vehicle vehicle, string userEmail);
    Task<bool> Update(Vehicle vehicle, string userEmail);
    Task<bool> SoftDelete(Guid id, string userEmail);
}
