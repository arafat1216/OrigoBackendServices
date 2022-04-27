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
        CreateMap<CustomerLabel, CustomerLabelDTO>();
        CreateMap<LifeCycleSetting, LifeCycleSettingDTO>();
        CreateMap<CategoryLifeCycleSetting, CategoryLifeCycleSettingDTO>();
        CreateMap<Label, LabelDTO>();
        CreateMap<AssetLifecycle, AssetLifecycleDTO>().ForMember(dest => dest.ContractHolderUserId,
            opts => opts.MapFrom(src => src.ContractHolderUser!.ExternalId));
        CreateMap<PagedModel<AssetLifecycle>, PagedModel<AssetLifecycleDTO>>();
    }
}