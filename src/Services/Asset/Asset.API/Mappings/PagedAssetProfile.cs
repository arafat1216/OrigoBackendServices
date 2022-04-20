using Asset.API.ViewModels;
using AssetServices.ServiceModel;
using AutoMapper;
using Common.Interfaces;

namespace Asset.API.Mappings;

public class PagedAssetProfile : Profile
{
    public PagedAssetProfile()
    {
        CreateMap<PagedModel<AssetLifecycleDTO>, PagedAssetList>().ForMember(dest => dest.Items, opts => opts.MapFrom(src => src.Items));
        CreateMap<CustomerLabelDTO, Label>()
            .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.Label.Text))
            .ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Label.Color))
            .ForMember(dest => dest.ColorName, opts => opts.MapFrom(src => src.Label.Color.ToString()));
        CreateMap<AssetLifecycleDTO, ViewModels.Asset>()
            .ForMember(dest => dest.Id, opts => opts.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.OrganizationId, opts => opts.MapFrom(src => src.CustomerId))
            .ForMember(dest => dest.AssetTag, opts => opts.MapFrom(src => string.Empty))
            .ForMember(dest => dest.ProductName, opts => opts.MapFrom(src => src.Asset.ProductName))
            .ForMember(dest => dest.Brand, opts => opts.MapFrom(src => src.Asset.Brand))
            .ForMember(dest => dest.LifecycleType, opts => opts.MapFrom(src => src.AssetLifecycleType))
            .ForMember(dest => dest.SerialNumber, opts => opts.MapFrom(src => src.Asset.SerialNumber))
            .ForMember(dest => dest.Imei, opts => opts.MapFrom(src => src.Asset.Imeis))
            .ForMember(dest => dest.MacAddress, opts => opts.MapFrom(src => src.Asset.MacAddress))
            .ForMember(dest => dest.ManagedByDepartmentId, opts => opts.MapFrom(src => src.ManagedByDepartmentId))
            .ForMember(dest => dest.AssetHolderId, opts => opts.MapFrom(src => src.ContractHolderUserId))
            .ForMember(dest => dest.AssetStatus, opts => opts.MapFrom(src => src.AssetLifecycleStatus))
            .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.Labels))
            .ForMember(dest => dest.AssetCategoryId, opts => opts.MapFrom(src => src.AssetCategoryId))
            .ForMember(dest => dest.BookValue, opts => opts.MapFrom(src => src.BookValue))
            .ForMember(dest => dest.BuyoutPrice, opts => opts.MapFrom(src => src.BuyoutPrice));
    }
}