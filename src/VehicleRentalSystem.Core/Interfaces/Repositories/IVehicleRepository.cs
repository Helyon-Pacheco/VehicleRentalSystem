using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByPlate(string plate);
    Task<IEnumerable<Vehicle>> GetAllByYear(int year);
}
