using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<OrganizationDTO, Organization>();
        }
    }
}
