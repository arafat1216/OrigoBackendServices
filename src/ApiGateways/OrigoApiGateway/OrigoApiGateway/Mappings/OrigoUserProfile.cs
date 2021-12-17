using AutoMapper;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;

namespace OrigoApiGateway.Mappings
{
    public class OrigoUserProfile : Profile
    {
        public OrigoUserProfile()
        {
            CreateMap<UserDTO, OrigoUser>()
                .ForMember(destination => destination.DisplayName, opt => opt.MapFrom(src => src.FirstName + " " + src.LastName))
                .ForMember(destination => destination.UserPreference, opt => opt.NullSubstitute(new UserPreferenceDTO{Language = string.Empty}));
        }
    }
}