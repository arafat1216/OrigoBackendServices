using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoUserPermissionProfile : Profile
    {
        public OrigoUserPermissionProfile()
        {
            CreateMap<UserPermissionsDTO, OrigoUserPermissions>();
        }
    }
}
