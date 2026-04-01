using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Services;

public interface IRentalService
{
    Task<Rental?> GetById(Guid id);
    Task<IEnumerable<Rental>> GetAll();
    Task<PaginatedResponse<Rental>> GetAllPaged(int page, int pageSize);
    Task<IEnumerable<Rental>> GetByCourierId(Guid courierId);
    Task<IEnumerable<Rental>> GetByVehicleId(Guid vehicleId);
    Task<IEnumerable<Rental>> GetActiveRentals();
    decimal CalculateRentalCost(Rental rental);
    Task<bool> Add(Rental rental, string userEmail);
    Task<bool> Update(Rental rental, string userEmail);
    Task<bool> SoftDelete(Guid id, string userEmail);
}
