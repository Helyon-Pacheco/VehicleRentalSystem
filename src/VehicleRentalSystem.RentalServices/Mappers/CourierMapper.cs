using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Dtos;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.RentalServices.Contracts.Request;

namespace VehicleRentalSystem.RentalServices.Mappers;

[ExcludeFromCodeCoverage]
public class CourierMapper : Profile
{
    public CourierMapper()
    {
        CreateMap<CourierDto, CourierRequest>().ReverseMap();
        CreateMap<CourierDto, CourierUpdateRequest>().ReverseMap();
        CreateMap<Courier, CourierRequest>().ReverseMap();
        CreateMap<Courier, CourierUpdateRequest>().ReverseMap();
        CreateMap<Courier, CourierDto>().ReverseMap();

        CreateMap<PaginatedResponse<Courier>, PaginatedResponse<CourierDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));
    }
}
