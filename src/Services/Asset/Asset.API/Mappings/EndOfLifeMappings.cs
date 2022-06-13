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
                .ForMember(destination => destination.ContractHolder, opt => opt.MapFrom(src => src.ContractHolder ?? null))
                .ForMember(destination => destination.AssetLifeCycleId, opt => opt.MapFrom(src => src.AssetLifeCycleId))
                .ForMember(destination => destination.Managers, opt => opt.MapFrom(src => src.Managers))
                .ForMember(destination => destination.CustomerAdmins, opt => opt.MapFrom(src => src.CustomerAdmins));
        }
    }
}
