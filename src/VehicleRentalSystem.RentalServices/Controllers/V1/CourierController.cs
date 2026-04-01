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
[Route("api/v{version:apiVersion}/couriers")]
public class CourierController : MainController
{
    private readonly ICourierService _courierService;
    private readonly IMapper _mapper;
    private readonly IRedisCacheService _redisCacheService;

    public CourierController(ICourierService courierService,
                             IMapper mapper,
                             IRedisCacheService redisCacheService,
                             INotifier notifier,
                             IAspNetUser user) : base(notifier, user)
    {
        _courierService = courierService;
        _mapper = mapper;
        _redisCacheService = redisCacheService;
    }

    [HttpGet("{id:guid}")]
    [ClaimsAuthorize("Courier", "Get")]
    public async Task<IActionResult> GetCourierById(Guid id)
    {
        var cacheKey = $"Courier:{id}";
        var cachedCourier = await _redisCacheService.GetCacheValueAsync<CourierDto>(cacheKey);
        if (cachedCourier != null)
        {
            return CustomResponse(cachedCourier);
        }

        return await HandleRequestAsync(
            async () =>
            {
                var courier = await _courierService.GetById(id);
                if (courier == null)
                {
                    NotifyError("Resource not found");
                    return CustomResponse(null, StatusCodes.Status404NotFound);
                }
                var courierDto = _mapper.Map<CourierDto>(courier);
                await _redisCacheService.SetCacheValueAsync(cacheKey, courierDto);
                return CustomResponse(courierDto);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [AllowAnonymous]
    [HttpGet("list")]
    public async Task<IActionResult> GetAllCouriers([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        return await HandleRequestAsync(
            async () =>
            {
                string cacheKey = "CourierList:All";

                if (page.HasValue && pageSize.HasValue)
                {
                    var cachedCouriers = await _redisCacheService.GetCacheValueAsync<PaginatedResponse<CourierDto>>(cacheKey);
                    if (cachedCouriers != null)
                    {
                        return CustomResponse(cachedCouriers);
                    }

                    var couriers = await _courierService.GetAllPaged(page.Value, pageSize.Value);
                    var courierDtos = _mapper.Map<PaginatedResponse<CourierDto>>(couriers);
                    await _redisCacheService.SetCacheValueAsync(cacheKey, courierDtos);
                    return CustomResponse(courierDtos);
                }
                else
                {
                    var cachedCouriers = await _redisCacheService.GetCacheValueAsync<IEnumerable<CourierDto>>(cacheKey);
                    if (cachedCouriers != null)
                    {
                        return CustomResponse(cachedCouriers);
                    }

                    var couriers = await _courierService.GetAll();
                    var courierDtos = _mapper.Map<IEnumerable<CourierDto>>(couriers);
                    await _redisCacheService.SetCacheValueAsync(cacheKey, courierDtos);
                    return CustomResponse(courierDtos);
                }
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [RequestSizeLimit(40000000)]
    [HttpPost]
    [ClaimsAuthorize("Courier", "Add")]
    public async Task<IActionResult> CreateCourier([FromForm] CourierRequest courierDto)
    {
        if (UserEmail == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                var courier = _mapper.Map<Courier>(courierDto);
                var success = await _courierService.Add(courier, UserEmail);
                if (!success)
                {
                    return CustomResponse("Resource conflict", StatusCodes.Status400BadRequest);
                }
                var createdCourierDto = _mapper.Map<CourierDto>(courier);

                return CustomResponse(createdCourierDto, StatusCodes.Status201Created);
            },
            ex => CustomResponse(ex.Message, StatusCodes.Status400BadRequest)
        );
    }

    [RequestSizeLimit(40000000)]
    [HttpPut("{id:guid}")]
    [ClaimsAuthorize("Courier", "Update")]
    public async Task<IActionResult> UpdateCourier(Guid id, [FromForm] CourierUpdateRequest courierDto)
    {
        if (UserEmail == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                var courier = _mapper.Map<Courier>(courierDto);
                courier.Id = id;
                await _courierService.Update(courier, UserEmail);

                var updatedCourierDto = _mapper.Map<CourierDto>(courier);

                return CustomResponse(updatedCourierDto, StatusCodes.Status204NoContent);
            },
            ex => CustomResponse(ex.Message)
        );
    }

    [HttpPatch("{id:guid}")]
    [ClaimsAuthorize("Courier", "Delete")]
    public async Task<IActionResult> SoftDeleteCourier(Guid id)
    {
        if (UserEmail == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                await _courierService.SoftDelete(id, UserEmail);

                return CustomResponse(null, StatusCodes.Status204NoContent);
            },
            ex => CustomResponse(ex.Message)
        );
    }

    [RequestSizeLimit(40000000)]
    [HttpPatch("{cnpj}/cnh")]
    [ClaimsAuthorize("Courier", "Update")]
    public async Task<IActionResult> AddOrUpdateCnhImage(string cnpj, IFormFile cnhImage)
    {
        if (UserEmail == null)
            return CustomResponse("User is not authenticated.", StatusCodes.Status401Unauthorized);

        return await HandleRequestAsync(
            async () =>
            {
                using (var stream = cnhImage.OpenReadStream())
                {
                    var result = await _courierService.AddOrUpdateCnhImage(cnpj, stream, UserEmail);
                    if (!result)
                    {
                        return CustomResponse("Failed to update CNH image", StatusCodes.Status400BadRequest);
                    }
                }

                return CustomResponse("CNH image updated successfully", StatusCodes.Status204NoContent);
            },
            ex => CustomResponse(ex.Message)
        );
    }
}
