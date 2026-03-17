using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Interfaces.Messaging;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Interfaces.UoW;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Core.Models.Enums;
using VehicleRentalSystem.Core.Models.Validations;
using VehicleRentalSystem.Core.Notifications;
using VehicleRentalSystem.Messaging.Events;

namespace VehicleRentalSystem.RentalServices.Services;

public class VehicleService : BaseService, IVehicleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageProducer _messageProducer;
    private readonly IRedisCacheService _redisCacheService;

    public VehicleService(IUnitOfWork unitOfWork, IMessageProducer messageProducer, INotifier notifier, IRedisCacheService redisCacheService) : base(notifier)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
    }

    public async Task<Vehicle> GetById(Guid id)
    {
        try
        {
            _notifier.Handle("Getting vehicle by ID");
            return await _unitOfWork.Vehicles.GetById(id);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<IEnumerable<Vehicle>> GetAll()
    {
        try
        {
            _notifier.Handle("Getting all vehicles");
            return await _unitOfWork.Vehicles.GetAll();
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<PaginatedResponse<Vehicle>> GetAllPaged(int page, int pageSize)
    {
        try
        {
            _notifier.Handle("Getting paged vehicles");
            return await _unitOfWork.Vehicles.GetAllPaged(page, pageSize);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<Vehicle> GetByPlate(string plate)
    {
        try
        {
            _notifier.Handle("Getting vehicle by plate");
            return await _unitOfWork.Vehicles.GetByPlate(plate);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<Vehicle> GetByType(VehicleType vehicleType)
    {
        try
        {
            _notifier.Handle("Getting vehicle by type");
            return await _unitOfWork.Vehicles.GetByType(vehicleType);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<IEnumerable<Vehicle>> GetAllByYear(int year)
    {
        try
        {
            _notifier.Handle("Getting all vehicles by year");
            return await _unitOfWork.Vehicles.GetAllByYear(year);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<VehicleNotification> GetVehicleNotification(Guid id)
    {
        try
        {
            _notifier.Handle("Getting vehicle notification by ID");
            return await _unitOfWork.VehicleNotifications.GetByVehicleId(id);
        }
        catch (Exception ex)
        {
            HandleException(ex);
            throw;
        }
    }

    public async Task<bool> Add(Vehicle vehicle, string userEmail)
    {
        if (vehicle == null)
        {
            _notifier.Handle("Vehicle details cannot be null", NotificationType.Error);
            return false;
        }

        var validator = new VehicleValidation(_unitOfWork);
        validator.ConfigureRulesForCreate();

        var validationResult = await validator.ValidateAsync(vehicle);
        if (!validationResult.IsValid)
        {
            _notifier.NotifyValidationErrors(validationResult);
            return false;
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                vehicle.CreatedByUser = userEmail;

                await _unitOfWork.Vehicles.Add(vehicle);
                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _notifier.Handle("Vehicle added successfully");

                    PublishVehicleRegisteredEvent(vehicle);
                    await _redisCacheService.RemoveCacheValueAsync("VehicleList:All");

                    return true;
                }

                await transaction.RollbackAsync();
                _notifier.Handle("Failed to add vehicle, rolling back transaction", NotificationType.Error);
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

    public async Task<bool> Update(Vehicle vehicle, string userEmail)
    {
        if (vehicle == null)
        {
            _notifier.Handle("Vehicle details cannot be null", NotificationType.Error);
            return false;
        }

        var existingVehicle = await _unitOfWork.Vehicles.GetById(vehicle.Id);
        if (existingVehicle == null)
        {
            _notifier.Handle("Vehicle not found", NotificationType.Error);
            return false;
        }

        var validator = new VehicleValidation(_unitOfWork);
        validator.ConfigureRulesForUpdate(existingVehicle);

        var validationResult = await validator.ValidateAsync(vehicle);
        if (!validationResult.IsValid)
        {
            _notifier.NotifyValidationErrors(validationResult);
            return false;
        }

        using (var transaction = await _unitOfWork.BeginTransactionAsync())
        {
            try
            {
                UpdateVehicleDetails(existingVehicle, vehicle, userEmail);

                await _unitOfWork.Vehicles.Update(existingVehicle);
                var result = await _unitOfWork.SaveAsync();

                if (result > 0)
                {
                    await transaction.CommitAsync();
                    _notifier.Handle("Vehicle updated successfully");

                    PublishVehicleRegisteredEvent(existingVehicle);
                    var cacheKey = $"Vehicle:{vehicle.Id}";
                    await _redisCacheService.RemoveCacheValueAsync(cacheKey);
                    await _redisCacheService.RemoveCacheValueAsync("VehicleList:All");

                    return true;
                }

                await transaction.RollbackAsync();
                _notifier.Handle("Failed to update vehicle, rolling back transaction", NotificationType.Error);
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
        try
        {
            if (id == Guid.Empty)
            {
                _notifier.Handle("Invalid vehicle ID", NotificationType.Error);
                return false;
            }

            var vehicle = await _unitOfWork.Vehicles.GetById(id);
            if (vehicle == null)
            {
                _notifier.Handle("Vehicle not found", NotificationType.Error);
                return false;
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    vehicle.UpdatedByUser = userEmail;

                    vehicle.ToggleIsDeleted();
                    await _unitOfWork.Vehicles.Update(vehicle);
                    var result = await _unitOfWork.SaveAsync();

                    if (result > 0)
                    {
                        await transaction.CommitAsync();
                        _notifier.Handle("Vehicle soft deleted successfully");

                        var cacheKey = $"Vehicle:{id}";
                        await _redisCacheService.RemoveCacheValueAsync(cacheKey);
                        await _redisCacheService.RemoveCacheValueAsync("VehicleList:All");

                        return true;
                    }

                    await transaction.RollbackAsync();
                    _notifier.Handle("Failed to soft delete vehicle, rolling back transaction", NotificationType.Error);
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
        catch (Exception ex)
        {
            HandleException(ex);
            return false;
        }
    }

    private void UpdateVehicleDetails(Vehicle existingVehicle, Vehicle updatedVehicle, string userEmail)
    {
        existingVehicle.Year = updatedVehicle.Year;
        existingVehicle.Model = updatedVehicle.Model;
        existingVehicle.Plate = updatedVehicle.Plate;
        existingVehicle.VehicleType = updatedVehicle.VehicleType;
        existingVehicle.UpdatedByUser = userEmail;
        existingVehicle.Update();
    }

    private void PublishVehicleRegisteredEvent(Vehicle vehicle)
    {
        var vehicleRegisteredEvent = new VehicleRegistered
        {
            Id = vehicle.Id,
            Year = vehicle.Year,
            Model = vehicle.Model,
            Plate = vehicle.Plate,
            VehicleType = vehicle.VehicleType,
            CreatedAt = vehicle.CreatedAt,
            CreatedByUser = vehicle.CreatedByUser,
            UpdatedAt = vehicle.UpdatedAt,
            UpdatedByUser = vehicle.UpdatedByUser,
            IsDeleted = vehicle.IsDeleted
        };
        _messageProducer.PublishAsync(vehicleRegisteredEvent, "vehicle_exchange", "vehicle_routingKey");
    }
}
