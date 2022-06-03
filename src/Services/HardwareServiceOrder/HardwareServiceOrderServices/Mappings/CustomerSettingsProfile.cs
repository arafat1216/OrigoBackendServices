using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class CustomerSettingsProfile : Profile
    {
        public CustomerSettingsProfile()
        {
            CreateMap<CustomerSettings, CustomerSettingsDTO>()
                .ForMember(d => d.ProviderId, opts => opts.Ignore())
                .ForMember(d => d.ServiceId, opts => opts.Ignore())
                .ForMember(d => d.AssetCategoryIds, opts => opts.Ignore());
        }
    }
}
