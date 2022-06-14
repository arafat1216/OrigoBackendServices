using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class CustomerSettingsProfile : Profile
    {
        public CustomerSettingsProfile()
        {
            //TODO: nullable properties should be handled later
            CreateMap<CustomerSettings, CustomerSettingsDTO>();
        }
    }
}
