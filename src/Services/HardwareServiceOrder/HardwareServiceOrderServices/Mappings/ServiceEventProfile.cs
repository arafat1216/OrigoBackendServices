using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HardwareServiceOrderServices.Mappings
{
    public class ServiceEventProfile : Profile
    {
        public ServiceEventProfile()
        {
            CreateMap<ExternalServiceEventDTO, ServiceEvent>();
        }
    }
}
