using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Dtos;
using VehicleRentalSystem.Core.Interfaces.Identity;
using VehicleRentalSystem.Core.Interfaces.Notifications;
using VehicleRentalSystem.Core.Interfaces.Services;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.Identity.Extensions;
using VehicleRentalSystem.RentalServices.Contracts.Request;

namespace VehicleRentalSystem.RentalServices.Controllers.V1;

[Authorize]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/vehicles")]
public class VehicleController : MainController
{
    private readonly IVehicleService _vehicleService;
    private readonly IMapper _mapper;
    private readonly IRedisCacheService _redisCacheService;

    public VehicleController(IVehicleService vehicleService,
                                IMapper mapper,
                                IRedisCacheService redisCacheService,
                                INotifier notifier,
                                IAspNetUser user) : base(notifier, user)
    {
        _vehicleService = vehicleService;
        _mapper = mapper;
        _redisCacheService = redisCacheService;
    }

    [HttpGet("{id:guid}")]
    [ClaimsAuthorize("Vehicle", "Get")]
    public async Task<IActionResult> GetVehicleById(Guid id)
    {
        var cacheKey = $"Vehicle:{id}";
        var cachedVehicle = await _redisCacheService.GetCacheValueAsync<VehicleDto>(cacheKey);
        if (cachedVehicle != null)
        {
            return CustomResponse(cachedVehicle);
        }

        return await HandleRequestAsync(
            async () =>
            {
                var vehicle = await _vehicleService.GetById(id);
                if (vehicle == null)
                {
                    return CustomResponse("Resource not found", StatusCodes.Status404NotFound);
                }
                var vehicleDto = _mapper.Map<VehicleDto>(vehicle);
                await _redisCacheService.SetCacheValueAsync(cacheKey, vehicleDto);
                return CustomResponse(vehicleDto);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [AllowAnonymous]
    [HttpGet("list")]
    public async Task<IActionResult> GetAllVehicles([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        return await HandleRequestAsync(
            async () =>
            {
                string cacheKey = "VehicleList:All";

                if (page.HasValue && pageSize.HasValue)
                {
                    var cachedVehicles = await _redisCacheService.GetCacheValueAsync<PaginatedResponse<VehicleDto>>(cacheKey);
                    if (cachedVehicles != null)
                    {
                        return CustomResponse(cachedVehicles);
                    }

                    var vehicles = await _vehicleService.GetAllPaged(page.Value, pageSize.Value);
                    var vehicleDtos = _mapper.Map<PaginatedResponse<VehicleDto>>(vehicles);
                    await _redisCacheService.SetCacheValueAsync(cacheKey, vehicleDtos);
                    return CustomResponse(vehicleDtos);
                }
                else
                {
                    var cachedVehicles = await _redisCacheService.GetCacheValueAsync<IEnumerable<VehicleDto>>(cacheKey);
                    if (cachedVehicles != null)
                    {
                        return CustomResponse(cachedVehicles);
                    }

                    var vehicles = await _vehicleService.GetAll();
                    var vehicleDtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
                    await _redisCacheService.SetCacheValueAsync(cacheKey, vehicleDtos);
                    return CustomResponse(vehicleDtos);
                }
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [HttpGet("{id:guid}/notification")]
    [ClaimsAuthorize("Vehicle", "Get")]
    public async Task<IActionResult> GetVehicleNotification(Guid id)
    {
        var cacheKey = $"VehicleNotification:{id}";
        var cachedNotification = await _redisCacheService.GetCacheValueAsync<VehicleNotificationDto>(cacheKey);
        if (cachedNotification != null)
        {
            return CustomResponse(cachedNotification);
        }

        return await HandleRequestAsync(
            async () =>
            {
                var notification = await _vehicleService.GetVehicleNotification(id);
                if (notification == null)
                {
                    return CustomResponse("Resource not found", StatusCodes.Status404NotFound);
                }
                var notificationDto = _mapper.Map<VehicleNotificationDto>(notification);
                await _redisCacheService.SetCacheValueAsync(cacheKey, notificationDto);
                return CustomResponse(notificationDto);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [HttpPost]
    [ClaimsAuthorize("Vehicle", "Add")]
    public async Task<IActionResult> CreateVehicle(VehicleRequest vehicleDto)
    {
        if (UserEmail == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                var vehicle = _mapper.Map<Vehicle>(vehicleDto);
                var result = await _vehicleService.Add(vehicle, UserEmail);
                if (!result)
                {
                    return CustomResponse("Resource conflict", StatusCodes.Status400BadRequest);
                }
                var createdVehicleDto = _mapper.Map<VehicleDto>(vehicle);

                return CustomResponse(createdVehicleDto, StatusCodes.Status201Created);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [HttpPut("{id:guid}")]
    [ClaimsAuthorize("Vehicle", "Update")]
    public async Task<IActionResult> UpdateVehicle(Guid id, VehicleUpdateRequest vehicleDto)
    {
        if (UserEmail  == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                var vehicle = _mapper.Map<Vehicle>(vehicleDto);
                vehicle.Id = id;
                await _vehicleService.Update(vehicle, UserEmail);
                var updatedVehicleDto = _mapper.Map<VehicleDto>(vehicle);

                return CustomResponse(updatedVehicleDto, StatusCodes.Status204NoContent);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [HttpPatch("{id:guid}/status")]
    [ClaimsAuthorize("Vehicle", "Delete")]
    public async Task<IActionResult> SoftDeleteVehicle(Guid id)
    {
        if (UserEmail == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                await _vehicleService.SoftDelete(id, UserEmail);

                return CustomResponse(null, StatusCodes.Status204NoContent);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }
}
