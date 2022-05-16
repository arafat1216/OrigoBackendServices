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
        /// Between service models and viewmodels
        /// </summary>
        public UsersPermissionsProfile()
        {
            CreateMap<UsersPermissionsDTO, UsersPermissions>();
            CreateMap<NewUserPermissionDTO, UserPermission>();

        }
    }
}
