using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Repositories;

public interface IRentalRepository : IRepository<Rental>
{
    Task<IEnumerable<Rental>> GetByCourierId(Guid courierId);
    Task<IEnumerable<Rental>> GetByMotorcycleId(Guid motorcycleId);
    Task<IEnumerable<Rental>> GetActiveRentals();
    decimal CalculateRentalCost(Rental rental);
}
