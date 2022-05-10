using System.Linq;
using AssetServices.Models;
using AssetServices.ServiceModel;
using AutoMapper;
using Common.Interfaces;

namespace AssetServices.Mappings;

public class AssetLifecycleProfile : Profile
{
    public AssetLifecycleProfile()
    {
        CreateMap<AssetLifecycle, MobilePhone>();
        CreateMap<User, UserDTO>();
        CreateMap<Asset, AssetDTO>();
        CreateMap<HardwareAsset, AssetDTO>();
        CreateMap<MobilePhone, AssetDTO>().IncludeBase<HardwareAsset, AssetDTO>().IncludeBase<Asset, AssetDTO>()
            .ForMember(dest => dest.Imeis, opts => opts.MapFrom(src => src.Imeis.Select(i => i.Imei).ToList()));
        CreateMap<Tablet, AssetDTO>().IncludeBase<HardwareAsset, AssetDTO>().IncludeBase<Asset, AssetDTO>()
            .ForMember(dest => dest.Imeis, opts => opts.MapFrom(src => src.Imeis.Select(i => i.Imei).ToList()));
        CreateMap<CustomerLabel, LabelDTO>()
            .ForMember(dest => dest.ExternalId, opts => opts.MapFrom(src => src.ExternalId))
            .ForMember(dest => dest.Text, opts => opts.MapFrom(src => src.Label.Text))
            .ForMember(dest => dest.Color, opts => opts.MapFrom(src => src.Label.Color));
        CreateMap<LifeCycleSetting, LifeCycleSettingDTO>();
        CreateMap<AssetLifecycle, AssetLifecycleDTO>()
            .ForMember(dest => dest.ContractHolderUserId,
            opts => opts.MapFrom(src => src.ContractHolderUser!.ExternalId))
            .ForMember(dest=> dest.Labels, opts => opts.MapFrom(src => src.Labels));
        CreateMap<PagedModel<AssetLifecycle>, PagedModel<AssetLifecycleDTO>>();
       

    }
}