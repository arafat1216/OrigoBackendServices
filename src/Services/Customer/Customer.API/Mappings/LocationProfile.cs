using AutoMapper;
using Customer.API.WriteModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    public class LocationProfile : Profile
    {
        public LocationProfile()
        {
            CreateMap<NewLocation, NewLocationDTO>();
        }
    }
}
