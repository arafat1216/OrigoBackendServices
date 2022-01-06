using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewUserProfile : Profile
    {
        public NewUserProfile()
        {
            CreateMap<NewUser, NewUserDTO>();
        }
    }
}
