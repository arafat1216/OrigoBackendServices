using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoContactPersonProfile : Profile
    {
        public OrigoContactPersonProfile()
        {
            CreateMap<OrigoContactPerson, ContactPersonDTO>();
        }
    }
}
