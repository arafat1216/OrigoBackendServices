using AutoMapper;
using Common.Enums;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SCIM;

namespace OrigoApiGateway.Mappings;

public class ScimUserProfile : Profile
{
    public ScimUserProfile()
    {
        CreateMap<ScimUser, NewUserDTO>()
            .ForMember(destination => destination.FirstName, options => options.MapFrom(source => source.Name.GivenName))
            .ForMember(destination => destination.LastName, options => options.MapFrom(source => source.Name.FamilyName))
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.Emails.FirstOrDefault()))
            .ForMember(destination => destination.MobileNumber, options => options.MapFrom(source => source.PhoneNumbers.FirstOrDefault() ?? string.Empty))
            .ForMember(destination => destination.EmployeeId, options => options.NullSubstitute(string.Empty))
            .ForMember(destination => destination.UserPreference, options => options.NullSubstitute(new UserPreferenceDTO { Language = string.Empty }))
            .ForMember(destination => destination.Role, options => options.NullSubstitute(PredefinedRole.EndUser));

        CreateMap<ScimUser, NewUser>()
            .ForMember(destination => destination.FirstName, options => options.MapFrom(source => source.Name.GivenName))
            .ForMember(destination => destination.LastName, options => options.MapFrom(source => source.Name.FamilyName))
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.Emails.FirstOrDefault()))
            .ForMember(destination => destination.MobileNumber, options => options.MapFrom(source => source.PhoneNumbers.FirstOrDefault() ?? string.Empty))
            .ForMember(destination => destination.EmployeeId, options => options.NullSubstitute(string.Empty))
            .ForMember(destination => destination.UserPreference, options => options.NullSubstitute(new UserPreferenceDTO { Language = string.Empty }))
            .ForMember(destination => destination.Role, options => options.NullSubstitute(PredefinedRole.EndUser));

        CreateMap<UserDTO, ScimUser>()
            .ForPath(destination => destination.Name.GivenName, options => options.MapFrom(source => source.FirstName))
            .ForPath(destination => destination.Name.FamilyName, options => options.MapFrom(source => source.LastName))
            .AfterMap((source, destination) => { destination.PhoneNumbers.Add(source.MobileNumber); })
            .AfterMap((source, destination) => { destination.Emails.Add(source.Email); });

        CreateMap<ScimUser, UserDTO>()
            .ForMember(destination => destination.FirstName, options => options.MapFrom(source => source.Name.GivenName))
            .ForMember(destination => destination.LastName, options => options.MapFrom(source => source.Name.FamilyName))
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.Emails.FirstOrDefault()))
            .ForMember(destination => destination.MobileNumber, options => options.MapFrom(source => source.PhoneNumbers.FirstOrDefault() ?? string.Empty));
            //.ForMember(destination => destination.Role, options => options.NullSubstitute(PredefinedRole.EndUser));
    }
}
