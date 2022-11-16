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
            .ForMember(destination => destination.Role, options => options.MapFrom(source => PredefinedRole.EndUser));

        CreateMap<ScimUser, NewUser>()
            .ForMember(destination => destination.FirstName, options => options.MapFrom(source => source.Name.GivenName))
            .ForMember(destination => destination.LastName, options => options.MapFrom(source => source.Name.FamilyName))
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.Emails.FirstOrDefault()))
            .ForMember(destination => destination.MobileNumber, options => options.MapFrom(source => source.PhoneNumbers.FirstOrDefault() ?? string.Empty))
            .ForMember(destination => destination.EmployeeId, options => options.NullSubstitute(string.Empty))
            .ForMember(destination => destination.UserPreference, options => options.NullSubstitute(new UserPreferenceDTO { Language = string.Empty }))
            .ForMember(destination => destination.Role, options => options.MapFrom(source => PredefinedRole.EndUser));
        
        CreateMap<ScimUser, OrigoUpdateUser>()
            .ForMember(destination => destination.FirstName, options => options.MapFrom(source => source.Name.GivenName))
            .ForMember(destination => destination.LastName, options => options.MapFrom(source => source.Name.FamilyName))
            .ForMember(destination => destination.Email, options => options.MapFrom(source => source.Emails.FirstOrDefault()))
            .ForMember(destination => destination.MobileNumber, options => options.MapFrom(source => source.PhoneNumbers.FirstOrDefault() ?? string.Empty))
            .ForMember(destination => destination.EmployeeId, options => options.NullSubstitute(string.Empty));

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

        CreateMap<OrigoUser, ScimUser>()
            .ForPath(destination => destination.ExternalId, options => options.MapFrom(source => source.Id))
            .ForPath(destination => destination.UserName, options => options.MapFrom(source => source.Email))
            .ForPath(destination => destination.Locale, options => options.MapFrom(source => source.UserPreference.Language))
            .ForPath(destination => destination.Name.GivenName, options => options.MapFrom(source => source.FirstName))
            .ForPath(destination => destination.Name.FamilyName, options => options.MapFrom(source => source.LastName))
            .ForPath(destination => destination.DisplayName, options => options.MapFrom(source => source.DisplayName))
            .AfterMap((source, destination) => { destination.Emails.Add(source.Email); })
            .AfterMap((source, destination) => { destination.PhoneNumbers.Add(source.MobileNumber); })
            .ForPath(destination => destination.Meta.ResourceType, options => options.MapFrom(source => "User"));

        CreateMap<OrigoUser, Resource>()
            .ForMember(destination => destination.Id, options => options.MapFrom(source => source.Id))
            .ForMember(destination => destination.ExternalId, options => options.MapFrom(source => source.Id))
            .ForPath(destination => destination.Meta.ResourceType, options => options.MapFrom(source => "User"));

        CreateMap<UserDTO, Resource>()
            .ForMember(destination => destination.Id, options => options.MapFrom(source => source.Id))
            .ForMember(destination => destination.ExternalId, options => options.MapFrom(source => source.Id))
            .ForPath(destination => destination.Meta.ResourceType, options => options.MapFrom(source => "User"));

        CreateMap<UserDTO, User>()
            .ForMember(destination => destination.Id, options => options.MapFrom(source => source.Id))
            .ForMember(destination => destination.ExternalId, options => options.MapFrom(source => source.Id))
            .ForPath(destination => destination.Meta.ResourceType, options => options.MapFrom(source => "User"))
            .ForPath(destination => destination.Name.GivenName, options => options.MapFrom(source => source.FirstName))
            .ForPath(destination => destination.Name.FamilyName, options => options.MapFrom(source => source.LastName))
            .ForPath(destination => destination.UserName, options => options.MapFrom(source => source.Email))
            .AfterMap((source, destination) => { destination.Emails.Add(new Email() { Value = source.Email, Primary = true }); })
            .AfterMap((source, destination) => { destination.PhoneNumbers.Add(new PhoneNumber() { Value = source.MobileNumber}); });

    }
}
