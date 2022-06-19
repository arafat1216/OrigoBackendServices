using AutoMapper;
using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices.Mappings
{
    public class ContactDetailsProfile : Profile
    {
        public ContactDetailsProfile()
        {
            CreateMap<ContactDetails, ContactDetailsDTO>();
        }
    }
}
