using AutoMapper;
using Customer.API.ApiModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>();
        }
    }
}
