using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Models;

namespace VehicleRentalSystem.Core.Interfaces.Services;

public interface ICourierService
{
    Task<Courier?> GetById(Guid id);
    Task<IEnumerable<Courier>> GetAll();
    Task<PaginatedResponse<Courier>> GetAllPaged(int page, int pageSize);
    Task<Courier?> GetByCnpj(string cnpj);
    Task<Courier?> GetByCnhNumber(string cnhNumber);
    Task<bool> Add(Courier courier, string userEmail);
    Task<bool> Update(Courier courier, string userEmail);
    Task<bool> SoftDelete(Guid id, string userEmail);
    Task<bool> AddOrUpdateCnhImage(string cnpj, Stream cnhImageStream, string userEmail);
}
