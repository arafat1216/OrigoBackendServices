using AutoMapper;
using Customer.API.ViewModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    /// <summary>
    /// Mapping profile for user permissions 
    /// </summary>
    public class UsersPermissionsProfile : Profile
    {
        /// <summary>
        /// Between service models and view models
        /// </summary>
        public UsersPermissionsProfile()
        {
            CreateMap<UsersPermissionsAddedDTO, UsersPermissions>();
            CreateMap<UserPermissionsDTO, UserPermissions>();
            CreateMap<NewUserPermissionDTO, UserPermission>();
        }
    }
}
