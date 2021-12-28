using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class LabelProfile : Profile
    {
        public LabelProfile()
        {
            CreateMap<LabelDTO, Label>();
        }
    }
}
