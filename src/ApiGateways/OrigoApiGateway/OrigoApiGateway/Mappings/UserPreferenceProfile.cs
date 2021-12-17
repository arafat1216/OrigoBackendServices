using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class UserPreferenceProfile : Profile
    {
        public UserPreferenceProfile()
        {
            CreateMap<UserPreferenceDTO, UserPreference>();
        }
    }
}