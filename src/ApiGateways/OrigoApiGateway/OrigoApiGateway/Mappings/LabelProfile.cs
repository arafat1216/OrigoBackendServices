using AutoMapper;
using OrigoApiGateway.Models.BackendDTO;
using System.Reflection.Emit;

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
