using Asset.API.ViewModels;
using AssetServices.ServiceModel;
using AutoMapper;

namespace Asset.API.Mappings
{
    public class EndOfLifeMappings : Profile
    {
        public EndOfLifeMappings()
        {
            CreateMap<ReturnDevice, ReturnDeviceDTO>()
                .ForMember(destination => destination.CallerId, opt => opt.MapFrom(src => src.CallerId))
                .ForMember(destination => destination.ReturnLocationId, opt => opt.MapFrom(src => src.ReturnLocationId))
                .ForMember(destination => destination.ContractHolder, opt => opt.MapFrom(src => src.ContractHolder ?? null))
                .ForMember(destination => destination.AssetLifeCycleId, opt => opt.MapFrom(src => src.AssetLifeCycleId))
                .ForMember(destination => destination.Managers, opt => opt.MapFrom(src => src.Managers))
                .ForMember(destination => destination.CustomerAdmins, opt => opt.MapFrom(src => src.CustomerAdmins));

            CreateMap<ReportDevice, ReportDeviceDTO>()
                .ForMember(destination => destination.CallerId, opt => opt.MapFrom(src => src.CallerId))
                .ForMember(destination => destination.AssetLifeCycleId, opt => opt.MapFrom(src => src.AssetLifeCycleId))
                .ForMember(destination => destination.ContractHolderUser, opt => opt.MapFrom(src => src.ContractHolderUser ?? null))
                .ForMember(destination => destination.Managers, opt => opt.MapFrom(src => src.Managers ?? null))
                .ForMember(destination => destination.ReportCategory, opt => opt.MapFrom(src => src.ReportCategory))
                .ForMember(destination => destination.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(destination => destination.TimePeriodFrom, opt => opt.MapFrom(src => src.TimePeriodFrom))
                .ForMember(destination => destination.TimePeriodTo, opt => opt.MapFrom(src => src.TimePeriodTo))
                .ForMember(destination => destination.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(destination => destination.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(destination => destination.PostalCode, opt => opt.MapFrom(src => src.PostalCode))
                .ForMember(destination => destination.City, opt => opt.MapFrom(src => src.City));
        }
    }
}
