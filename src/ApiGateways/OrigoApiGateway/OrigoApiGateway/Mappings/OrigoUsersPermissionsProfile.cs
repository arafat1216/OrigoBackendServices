using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoUsersPermissionsProfile : Profile
    {
        public OrigoUsersPermissionsProfile()
        {
            CreateMap<NewUsersPermissions, NewUsersPermissionsDTO>();
        }
    }
}
