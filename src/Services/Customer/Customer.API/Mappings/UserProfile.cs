using AutoMapper;
using Customer.API.ViewModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserDTO, User>();
            CreateMap<CustomerServices.ServiceModels.UserInfo, ViewModels.UserInfo>();
        }
    }
}
