using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewUserPermissionsProfile : Profile
    {
        public NewUserPermissionsProfile()
        {
            CreateMap<NewUserPermissions,NewUserPermissionsDTO>();
        }
    }
}
