using AutoMapper;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

namespace CustomerServices.Mappings
{
    public class UsersPermissionsProfile : Profile
    {
        public UsersPermissionsProfile()
        {
            CreateMap<UserPermissions, NewUserPermissionDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(u => u.User.UserId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(r => r.Role.Name));
        }
    }
}
