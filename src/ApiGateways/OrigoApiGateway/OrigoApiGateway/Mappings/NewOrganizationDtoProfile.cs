using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class NewOrganizationDtoProfile : Profile
    {
        public NewOrganizationDtoProfile()
        {
            CreateMap<NewOrganization, NewOrganizationDTO>();
        }
    }
}
