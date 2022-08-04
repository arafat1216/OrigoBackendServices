using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class ServiceProviderProfile : Profile
    {
        public ServiceProviderProfile()
        {
            CreateMap<ServiceProvider, ServiceProviderDTO>();
        }
    }
}
