using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Interfaces.UoW;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Models.Validations;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Messaging.Events;

namespace VehicleRentalSystem.RentalServices.Services;

public class CourierService : BaseService, ICourierService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageProducer _messageProducer;
    private readonly IRedisCacheService _redisCacheService;

    public CourierService(IUnitOfWork unitOfWork, IMessageProducer messageProducer, INotifier notifier, IRedisCacheService redisCacheService) : base(notifier)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
    }

    public async Task<Courier> GetById(Guid id)
    {
        try
        {
            _notifier.Handle("Getting courier by ID");
            return await _unitOfWork.Couriers.GetById(id);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<IEnumerable<Courier>> GetAll()
    {
        try
        {
            _notifier.Handle("Getting all couriers");
            return await _unitOfWork.Couriers.GetAll();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<PaginatedResponse<Courier>> GetAllPaged(int page, int pageSize)
    {
        try
        {
            _notifier.Handle("Getting paged couriers");
            return await _unitOfWork.Couriers.GetAllPaged(page, pageSize);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<Courier> GetByCnpj(string cnpj)
    {
        try
        {
            _notifier.Handle("Getting courier by CNPJ");
            return await _unitOfWork.Couriers.GetByCnpj(cnpj);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<Courier> GetByCnhNumber(string cnhNumber)
    {
        try
        {
            _notifier.Handle("Getting courier by CNH number");
            return await _unitOfWork.Couriers.GetByCnhNumber(cnhNumber);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<bool> Add(Courier courier, string userEmail)
    {
        if (courier == null)
        {
            _notifier.Handle("Courier details cannot be null", NotificationType.Error);
            return false;
        }

        var validator = new CourierValidation(_unitOfWork);
        validator.ConfigureRulesForCreate();

        var validationResult = await validator.ValidateAsync(courier);
        if (!validationResult.IsValid)
        {
            _notifier.NotifyValidationErrors(validationResult);
            return false;
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                courier.CreatedByUser = userEmail;

                await _unitOfWork.Couriers.Add(courier);
                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _notifier.Handle("Courier added successfully");

                    PublishCourierRegisteredEvent(courier);
                    await _redisCacheService.RemoveCacheValueAsync("CourierList:All");

                    return true;
                }

                await transaction.RollbackAsync();
                _notifier.Handle("Failed to add courier, rolling back transaction", NotificationType.Error);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                HandleException(ex);
                return false;
            }
        }
    }

    public async Task<bool> Update(Courier courier, string userEmail)
    {
        if (courier == null)
        {
            _notifier.Handle("Courier details cannot be null", NotificationType.Error);
            return false;
        }

        var existingCourier = await _unitOfWork.Couriers.GetById(courier.Id);
        if (existingCourier == null)
        {
            _notifier.Handle("Courier not found", NotificationType.Error);
            return false;
        }

        var validator = new CourierValidation(_unitOfWork);
        validator.ConfigureRulesForUpdate(existingCourier);

        var validationResult = await validator.ValidateAsync(courier);
        if (!validationResult.IsValid)
        {
            _notifier.NotifyValidationErrors(validationResult);
            return false;
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                UpdateCourierDetails(existingCourier, courier, userEmail);

                await _unitOfWork.Couriers.Update(existingCourier);
                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _notifier.Handle("Courier updated successfully");

                    PublishCourierRegisteredEvent(courier);
                    var cacheKey = $"Courier:{courier.Id}";
                    await _redisCacheService.RemoveCacheValueAsync(cacheKey);
                    await _redisCacheService.RemoveCacheValueAsync("CourierList:All");

                    return true;
                }

                await transaction.RollbackAsync();
                _notifier.Handle("Failed to update courier, rolling back transaction", NotificationType.Error);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                HandleException(ex);
                return false;
            }
        }
    }

    public async Task<bool> SoftDelete(Guid id, string userEmail)
    {
        if (id == Guid.Empty)
        {
            _notifier.Handle("Invalid courier ID", NotificationType.Error);
            return false;
        }

        var courier = await _unitOfWork.Couriers.GetById(id);
        if (courier == null)
        {
            _notifier.Handle("Courier not found", NotificationType.Error);
            return false;
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                courier.ToggleIsDeleted();
                await _unitOfWork.Couriers.Update(courier);
                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _notifier.Handle("Courier soft deleted successfully");

                    var cacheKey = $"Courier:{id}";
                    await _redisCacheService.RemoveCacheValueAsync(cacheKey);
                    await _redisCacheService.RemoveCacheValueAsync("CourierList:All");

                    return true;
                }

                await transaction.RollbackAsync();
                _notifier.Handle("Failed to soft delete courier, rolling back transaction", NotificationType.Error);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                HandleException(ex);
                return false;
            }
        }
    }

    public async Task<bool> AddOrUpdateCnhImage(string cnpj, Stream cnhImageStream, string userEmail)
    {
        if (string.IsNullOrEmpty(cnpj))
        {
            _notifier.Handle("CNPJ cannot be null or empty", NotificationType.Error);
            return false;
        }

        if (cnhImageStream == null)
        {
            _notifier.Handle("CNH image stream cannot be null", NotificationType.Error);
            return false;
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                var courier = await _unitOfWork.Couriers.GetByCnpj(cnpj);
                if (courier == null)
                {
                    _notifier.Handle($"Courier with CNPJ {cnpj} not found.", NotificationType.Error);
                    await transaction.RollbackAsync();
                    return false;
                }

                courier.UpdatedByUser = userEmail;

                var cnhImageUrl = await _unitOfWork.Couriers.AddOrUpdateCnhImage(cnpj, cnhImageStream);
                courier.CnhImage = cnhImageUrl;

                var validator = new CourierValidation(_unitOfWork);
                var imageValidationResult = await validator.ValidateImageAsync(courier);
                if (!imageValidationResult.IsValid)
                {
                    await transaction.RollbackAsync();
                    _notifier.NotifyValidationErrors(imageValidationResult);
                    return false;
                }

                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _notifier.Handle("CNH image updated successfully");

                    var cacheKey = $"Courier:{courier.Id}";
                    await _redisCacheService.RemoveCacheValueAsync(cacheKey);
                    await _redisCacheService.RemoveCacheValueAsync("CourierList:All");

                    return true;
                }

                await transaction.RollbackAsync();
                _notifier.Handle("Failed to update CNH image, rolling back transaction", NotificationType.Error);
                return false;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                HandleException(ex);
                return false;
            }
        }
    }

    private void UpdateCourierDetails(Courier existingCourier, Courier newCourier, string userEmail)
    {
        existingCourier.Name = newCourier.Name;
        existingCourier.Cnpj = newCourier.Cnpj;
        existingCourier.BirthDate = newCourier.BirthDate;
        existingCourier.CnhNumber = newCourier.CnhNumber;
        existingCourier.CnhType = newCourier.CnhType;
        existingCourier.CnhImage = newCourier.CnhImage;
        existingCourier.UpdatedByUser = userEmail;
        existingCourier.Update();
    }

    private void PublishCourierRegisteredEvent(Courier courier)
    {
        var courierRegisteredEvent = new CourierRegistered
        {
            Id = courier.Id,
            Name = courier.Name,
            Cnpj = courier.Cnpj,
            BirthDate = courier.BirthDate,
            CnhNumber = courier.CnhNumber,
            CnhType = courier.CnhType,
            CnhImage = courier.CnhImage,
            CreatedAt = courier.CreatedAt,
            CreatedByUser = courier.CreatedByUser,
            UpdatedAt = courier.UpdatedAt,
            UpdatedByUser = courier.UpdatedByUser,
            IsDeleted = courier.IsDeleted
        };
        _messageProducer.PublishAsync(courierRegisteredEvent, "courier_exchange", "courier_routingKey");
    }
}
