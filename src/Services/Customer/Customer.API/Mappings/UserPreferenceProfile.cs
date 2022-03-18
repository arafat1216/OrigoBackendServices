using AutoMapper;
using Customer.API.ApiModels;
using CustomerServices.ServiceModels;

namespace Customer.API.Mappings
{
    public class UserPreferenceProfile : Profile
    {
        public UserPreferenceProfile()
        {
            CreateMap<UserPreferenceDTO, UserPreference>();
        }
    }
}