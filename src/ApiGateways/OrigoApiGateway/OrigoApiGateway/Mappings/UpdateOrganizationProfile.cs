using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class UpdateOrganizationProfile : Profile
    {
        public UpdateOrganizationProfile()
        {
            //Map from UpdateOrganization to UpdateOrganizationDTO
            CreateMap<UpdateOrganization, UpdateOrganizationDTO>();
        }
    }
}
