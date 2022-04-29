using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class CustomerSettingsProfile : Profile
    {
        public CustomerSettingsProfile()
        {
            CreateMap<CustomerSettings, CustomerSettingsDTO>();
        }
    }
}
