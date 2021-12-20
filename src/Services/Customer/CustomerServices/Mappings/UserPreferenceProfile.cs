using AutoMapper;
using CustomerServices.Models;
using CustomerServices.ServiceModels;

namespace CustomerServices.Mappings
{
    public class UserPreferenceDTOProfile : Profile
    {
        public UserPreferenceDTOProfile()
        {
            CreateMap<UserPreference, UserPreferenceDTO>();
        }
    }
}
