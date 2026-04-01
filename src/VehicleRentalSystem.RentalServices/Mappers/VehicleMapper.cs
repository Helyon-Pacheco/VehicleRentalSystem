using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Dtos;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.RentalServices.Contracts.Request;

namespace VehicleRentalSystem.RentalServices.Mappers;

[ExcludeFromCodeCoverage]
public class VehicleMapper : Profile
{
    public VehicleMapper()
    {
        CreateMap<VehicleDto, VehicleRequest>().ReverseMap();
        CreateMap<VehicleDto, VehicleUpdateRequest>().ReverseMap();
        CreateMap<Vehicle, VehicleRequest>().ReverseMap();
        CreateMap<Vehicle, VehicleUpdateRequest>().ReverseMap();
        CreateMap<Vehicle, VehicleDto>().ReverseMap();

        CreateMap<VehicleNotification, VehicleNotificationDto>().ReverseMap();

        CreateMap<PaginatedResponse<Vehicle>, PaginatedResponse<VehicleDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));
    }
}
