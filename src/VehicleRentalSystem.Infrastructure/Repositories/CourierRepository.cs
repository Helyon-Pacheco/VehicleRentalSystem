using Microsoft.EntityFrameworkCore;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Repositories;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Infrastructure.Context;

namespace VehicleRentalSystem.Infrastructure.Repositories;

public class CourierRepository : Repository<Courier>, ICourierRepository
{
    private readonly IBlobStorageService _blobStorageService;

    public CourierRepository(DataContext dataContext, INotifier notifier, IBlobStorageService blobStorageService)
        : base(dataContext, notifier)
    {
        _blobStorageService = blobStorageService;
    }

    public async Task<Courier> GetByCnpj(string cnpj)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(Courier)} by CNPJ {cnpj}.");
            return await _dbSet.FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(Courier)} by CNPJ {cnpj}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<Courier> GetByCnhNumber(string cnhNumber)
    {
        try
        {
            _notifier.Handle($"Getting {nameof(Courier)} by CNH Number {cnhNumber}.");
            return await _dbSet.FirstOrDefaultAsync(c => c.CnhNumber == cnhNumber);
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error getting {nameof(Courier)} by CNH Number {cnhNumber}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }

    public async Task<string> AddOrUpdateCnhImage(string cnpj, Stream cnhImageStream)
    {
        try
        {
            var courier = await GetByCnpj(cnpj);
            if (courier == null)
            {
                _notifier.Handle($"Courier with CNPJ {cnpj} not found.", NotificationType.Error);
                return null;
            }

            var cnhImageUrl = await _blobStorageService.UploadFileAsync(cnhImageStream, $"{courier.CnhNumber}.png");
            courier.CnhImage = cnhImageUrl;

            _dbSet.Update(courier);

            return cnhImageUrl;
        }
        catch (Exception ex)
        {
            _notifier.Handle($"Error updating CNH image for courier with CNPJ {cnpj}: {ex.Message}", NotificationType.Error);
            throw;
        }
    }
}
