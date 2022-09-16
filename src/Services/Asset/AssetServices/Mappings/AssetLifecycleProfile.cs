using System;
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
        CreateMap<Asset, AssetDTO>();
        CreateMap<User, UserDTO>();
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
        CreateMap<DisposeSetting, DisposeSettingDTO>();
        CreateMap<ReturnLocation, ReturnLocationDTO>();
        CreateMap<AssetLifecycle, AssetLifecycleDTO>()
            .ForMember(dest => dest.ContractHolderUserId,
                opts => opts.MapFrom(src => src.ContractHolderUser!.ExternalId))
            .ForMember(dest => dest.Labels, opts => opts.MapFrom(src => src.Labels)).ForMember(dest => dest.Source,
                opts => opts.MapFrom(src => src.Source.ToString()));
        CreateMap<PagedModel<AssetLifecycle>, PagedModel<AssetLifecycleDTO>>();
        CreateMap<NewAssetDTO, CreateAssetLifecycleDTO>().ForMember(dest => dest.Source, opts => opts.Ignore())
            .ForMember(dest => dest.Runtime, opts => opts.Ignore());
        CreateMap<ImportedAsset, NewAssetDTO>()
            .ForMember(dest => dest.UserEmail, opts => opts.MapFrom(src => src.ImportedUser.Email))
            .ForMember(dest => dest.UserLastName, opts => opts.MapFrom(src => src.ImportedUser.LastName))
            .ForMember(dest => dest.UserFirstName, opts => opts.MapFrom(src => src.ImportedUser.FirstName))
            .ForMember(dest => dest.UserPhoneNumber, opts => opts.MapFrom(src => src.ImportedUser.PhoneNumber))
            .ForMember(dest => dest.Description, opts => opts.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Note, opts => opts.MapFrom(src => string.Empty))
            .ForMember(dest => dest.Source, opts => opts.MapFrom(src => "FileImport"))
            .ForMember(dest => dest.Imei,
                opts => opts.MapFrom(src =>
                    src.Imeis.Split(",", StringSplitOptions.TrimEntries).Select(long.Parse).ToList()));
    }
}