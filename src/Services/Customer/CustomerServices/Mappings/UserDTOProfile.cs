using AutoMapper;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

namespace CustomerServices.Mappings
{
    public class UserDTOProfile : Profile
    {
        public UserDTOProfile()
        {
            CreateMap<User, UserDTO>()
                .ForMember(u => u.Id, opt => opt.MapFrom(src => src.UserId))
                .ForMember(u => u.OrganizationName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(u => u.UserStatusName, opt => opt.MapFrom(src => src.UserStatus.ToString()))
                .ForMember(u => u.UserStatus, opt => opt.MapFrom(src => src.UserStatus));
            ;
        }
    }
}