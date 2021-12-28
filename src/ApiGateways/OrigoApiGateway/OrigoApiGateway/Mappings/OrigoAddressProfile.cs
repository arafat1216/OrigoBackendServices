using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoAddressProfile : Profile
    {
        public OrigoAddressProfile()
        {
            CreateMap<AddressDTO, OrigoAddress>();
        }
    }
}
