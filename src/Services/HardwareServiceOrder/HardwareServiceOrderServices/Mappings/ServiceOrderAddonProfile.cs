using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class ServiceOrderAddonProfile : Profile
    {
        public ServiceOrderAddonProfile()
        {
            CreateMap<ServiceOrderAddon, ServiceOrderAddonDTO>();
        }
    }
}
