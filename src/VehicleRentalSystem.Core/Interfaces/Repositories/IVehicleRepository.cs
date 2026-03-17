using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Models.Enums;

namespace VehicleRentalSystem.Core.Interfaces.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByPlate(string plate);
    Task<Vehicle?> GetByType(VehicleType type);
    Task<IEnumerable<Vehicle>> GetAllByYear(int year);
}
