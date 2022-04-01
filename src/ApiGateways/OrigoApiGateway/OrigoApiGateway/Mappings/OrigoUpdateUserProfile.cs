using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoUpdateUserProfile : Profile
    {
        public OrigoUpdateUserProfile()
        {
            CreateMap<OrigoUpdateUser, UpdateUserDTO>();
        }
    }
}
