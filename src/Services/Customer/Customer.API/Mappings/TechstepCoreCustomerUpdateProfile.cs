using AutoMapper;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    public class TechstepCoreCustomerUpdateProfile : Profile
    {
        public TechstepCoreCustomerUpdateProfile()
        {
            CreateMap<TechstepCoreData, TechstepCoreDataDTO>();
            CreateMap<TechstepCoreCustomerUpdate, TechstepCoreCustomerUpdateDTO>();
        }
    }
}
