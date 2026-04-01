using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using VehicleRentalSystem.Core.Common;
using VehicleRentalSystem.Core.Dtos;
using VehicleRentalSystem.Core.Models;
using VehicleRentalSystem.RentalServices.Contracts.Request;

namespace VehicleRentalSystem.RentalServices.Mappers;

[ExcludeFromCodeCoverage]
public class RentalMapper : Profile
{
    public RentalMapper()
    {
        CreateMap<RentalDto, RentalRequest>().ReverseMap();
        CreateMap<RentalDto, RentalUpdateRequest>().ReverseMap();
        CreateMap<Rental, RentalRequest>().ReverseMap();
        CreateMap<Rental, RentalUpdateRequest>().ReverseMap();
        CreateMap<Rental, RentalDto>()
            .ForMember(dest => dest.Courier, opt => opt.MapFrom(src => src.Courier))
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
            .ReverseMap();

        CreateMap<PaginatedResponse<Rental>, PaginatedResponse<RentalDto>>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items))
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.PageNumber))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.PageSize))
            .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src => src.TotalItems))
            .ForMember(dest => dest.TotalPages, opt => opt.MapFrom(src => src.TotalPages));
    }
}
