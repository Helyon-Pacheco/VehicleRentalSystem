using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Repositories;

public interface ICourierRepository : IRepository<Courier>
{
    Task<Courier?> GetByCnpj(string cnpj);
    Task<Courier?> GetByCnhNumber(string cnhNumber);
    Task<string?> AddOrUpdateCnhImage(string cnpj, Stream cnhImageStream);
}
