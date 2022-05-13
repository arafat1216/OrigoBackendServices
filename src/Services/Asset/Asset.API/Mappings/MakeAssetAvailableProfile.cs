using System;
using System.Collections.Generic;
using Asset.API.ViewModels;
using AssetServices.ServiceModel;
using AutoMapper;

namespace Asset.API.Mappings
{
    public class MakeAssetAvailableProfile : Profile
    {
        public MakeAssetAvailableProfile()
        {
            CreateMap<MakeAssetAvailable, MakeAssetAvailableDTO>()
                .ForMember(destination => destination.CallerId, opt => opt.MapFrom(src => src.CallerId))
                .ForMember(destination => destination.PreviousUser, opt => opt.MapFrom(src => src.PreviousUser ?? null))
                .ForMember(destination => destination.AssetLifeCycleId, opt => opt.MapFrom(src => src.AssetLifeCycleId));
            CreateMap<EmailPersonAttribute, EmailPersonAttributeDTO>()
                .ForMember(destination => destination.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(destination => destination.Name, opt => opt.MapFrom(src => src.Name ?? null))
                .ForMember(destination => destination.PreferedLanguage, opt => opt.MapFrom(src => src.PreferedLanguage));
            CreateMap<ReAssignAsset, ReAssignAssetDTO>()
                .ForMember(destination => destination.Personal, opt => opt.MapFrom(src => src.Personal))
                .ForMember(destination => destination.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(destination => destination.PreviousUser, opt => opt.MapFrom(src => src.PreviousUser))
                .ForMember(destination => destination.NewUser, opt => opt.MapFrom(src => src.NewUser))
                .ForMember(destination => destination.PreviousManagers, opt => opt.MapFrom(src => src.PreviousManagers))
                .ForMember(destination => destination.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
                .ForMember(destination => destination.CallerId, opt => opt.MapFrom(src => src.CallerId));

        }
    }
}
