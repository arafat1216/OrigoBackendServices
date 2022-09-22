using System.Collections.Immutable;
using AutoMapper;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

namespace CustomerServices.Mappings;

public class UsersPermissionsProfile : Profile
{
    public UsersPermissionsProfile()
    {
        CreateMap<UserPermissionsDTO, NewUserPermissionDTO>();
        CreateMap<UserPermissions, UserPermissionsDTO>()
            .ForMember(dest => dest.AccessList, opt => opt.MapFrom(src => src.AccessList.ToImmutableList()))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.UserId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.Name));
        CreateMap<UserPermissions, NewUserPermissionDTO>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(u => u.User.UserId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(r => r.Role.Name));
    }
}