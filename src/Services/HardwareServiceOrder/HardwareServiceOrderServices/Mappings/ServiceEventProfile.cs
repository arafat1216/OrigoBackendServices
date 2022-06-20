using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class ServiceEventProfile : Profile
    {
        public ServiceEventProfile()
        {
            CreateMap<ExternalServiceEventDTO, ServiceEvent>();
            CreateMap<ServiceEvent, ExternalServiceEventDTO>();
        }
    }
}
