using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrganizationPreferencesProfile : Profile
    {
        public OrganizationPreferencesProfile()
        {
            CreateMap<NewOrganizationPreferences, OrganizationPreferencesDTO>();
        }
    }
}
