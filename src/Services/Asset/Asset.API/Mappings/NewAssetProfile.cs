using System;
using System.Collections.Generic;
using Asset.API.ViewModels;
using AssetServices.ServiceModel;
using AutoMapper;

namespace Asset.API.Mappings;

public class NewAssetProfile : Profile
{
    public NewAssetProfile()
    {
        CreateMap<NewAsset, NewAssetDTO>()
            .ForMember(destination => destination.Note, opt => opt.MapFrom(src => src.Note ?? string.Empty))
            .ForMember(destination => destination.Alias, opt => opt.MapFrom(src => src.Alias ?? string.Empty))
            .ForMember(destination => destination.Brand, opt => opt.MapFrom(src => src.Brand ?? string.Empty))
            .ForMember(destination => destination.ProductName, opt => opt.MapFrom(src => src.ProductName ?? string.Empty))
            .ForMember(destination => destination.AssetTag, opt => opt.MapFrom(src => src.AssetTag ?? string.Empty))
            .ForMember(destination => destination.Imei, opt => opt.MapFrom(src => src.Imei ?? new List<long>()))
            .ForMember(destination => destination.PaidByCompany, opt => opt.MapFrom(src => src.PaidByCompany ?? 0.0m))
            .ForMember(destination => destination.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty));
    }
}