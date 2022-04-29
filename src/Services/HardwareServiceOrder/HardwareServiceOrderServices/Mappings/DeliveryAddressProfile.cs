using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;
namespace HardwareServiceOrderServices.Mappings
{
    internal class DeliveryAddressProfile : Profile
    {
        public DeliveryAddressProfile()
        {
            CreateMap<DeliveryAddress, DeliveryAddressDTO>();
        }
    }
}
