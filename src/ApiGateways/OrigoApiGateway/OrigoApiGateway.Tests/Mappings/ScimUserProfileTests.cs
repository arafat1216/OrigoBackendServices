using System.Linq;
using System.Reflection;
using AutoMapper;
using Common.Enums;
using OrigoApiGateway.Mappings;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.BackendDTO;
using OrigoApiGateway.Models.SCIM;

namespace OrigoApiGateway.Tests.Mappings;

public class ScimUserProfileTests
{
    private readonly IMapper _mapper;

    public ScimUserProfileTests()
    {
        _mapper = new MapperConfiguration(config =>
        {
            config.AddMaps(Assembly.GetAssembly(typeof(ScimUserProfile)));
        }).CreateMapper();
    }

    private ScimUser GetScimUser()
    {
        return new ScimUser
        {
            UserName = "john",
            ExternalId = Guid.NewGuid().ToString(),
            Name = new()
            {
                Formatted = "Mr. John Doe",
                FamilyName = "Doe",
                GivenName = "John",
                HonorificPrefix = "Mr.",
                HonorificSuffix = null,
                MiddleName = "John",
            },
            DisplayName = "John Doe",
            NickName = "John",
            ProfileUrl = null,
            Title = "Junior Developer",
            UserType = "Employee",
            PreferredLanguage = null,
            Locale = "en-US",
            TimeZone = DateTime.Now.ToString(),
            Active = true,
            Emails = new List<string> { "john@test-domain.com" },
            PhoneNumbers = new List<string> { "+4759555724" },
            Groups = new List<string> { Guid.NewGuid().ToString() },
        };
    }

    private User GetUserResource()
    {
        return new User
        {
            UserName = "john@test-domain.com",
            Name = new()
            {
                FamilyName = "Doe",
                GivenName = "John",
                MiddleName = "John"
            },
            Active = true,
            Emails = new()
            {
                new() 
                {
                    Value = "john@test-domain.com",
                    Primary = true
                }
            },
            PhoneNumbers = new()
            {
                new()
                {
                    Value = "+4759555724"
                }
            }
        };
    }

    private OrigoUser GetOrigoUser()
    {
        return new OrigoUser
        {
            Id = Guid.NewGuid(),
            Email = "test@gmail.com",
            FirstName = "Test",
            LastName = "User",
            DisplayName = "test",
            Role = PredefinedRole.EndUser.ToString(),
            DepartmentName = "IT Department",
            EmployeeId = "123",
            MobileNumber = "+4759555724"
        };
    }

    private UserDTO GetUserDto()
    {
        return new UserDTO
        {
            Email = "john@test-domain.com",
            FirstName = "John",
            LastName = "Doe",
            MobileNumber = "+4759555724",
        };
    }

    [Fact]
    public void ScimUser_To_NewUserDTO_ProfileTest()
    {
        // Arrange
        var scimUser = GetScimUser();

        // Act
        var result = _mapper.Map<NewUserDTO>(scimUser);

        // Assert
        Assert.Equal(scimUser.Name.GivenName, result.FirstName);
        Assert.Equal(scimUser.Name.FamilyName, result.LastName);
        Assert.Equal(scimUser.Emails.FirstOrDefault(), result.Email);
        Assert.Equal(scimUser?.PhoneNumbers?.FirstOrDefault(), result.MobileNumber);
        Assert.Equal(PredefinedRole.EndUser.ToString(), result.Role);
        Assert.Equal(Guid.Empty, result.CallerId);
        Assert.Null(result.EmployeeId);
        Assert.Null(result.UserPreference);
        Assert.False(result.NeedsOnboarding);
    }
    
    [Fact]
    public void ScimUser_To_NewUser_Test()
    {
        // Arrange
        var scimUser = GetScimUser();

        // Act
        var result = _mapper.Map<NewUser>(scimUser);

        // Assert
        Assert.Equal(scimUser.Name.GivenName, result.FirstName);
        Assert.Equal(scimUser.Name.FamilyName, result.LastName);
        Assert.Equal(scimUser.Emails.FirstOrDefault(), result.Email);
        Assert.Equal(scimUser?.PhoneNumbers?.FirstOrDefault(), result.MobileNumber);
        Assert.Equal(PredefinedRole.EndUser.ToString(), result.Role);
        Assert.Null(result.EmployeeId);
        Assert.Null(result.UserPreference);
    }

    [Fact]
    public void UserDTO_To_ScimUser_ProfileTest()
    {
        // Arrange
        var userDto = GetUserDto();

        // Act
        var result = _mapper.Map<ScimUser>(userDto);

        // Assert
        Assert.Equal(userDto.FirstName, result.Name.GivenName);
        Assert.Equal(userDto.LastName, result.Name.FamilyName);
        Assert.Equal(userDto.Email, result.Emails.FirstOrDefault());
        Assert.Equal(userDto.MobileNumber, result?.PhoneNumbers?.FirstOrDefault());
    }

    [Fact]
    public void ScimUser_To_UserDTO_ProfileTest()
    {
        // Arrange
        var scimUser = GetScimUser();

        // Act
        var result = _mapper.Map<UserDTO>(scimUser);

        // Assert
        Assert.Equal(scimUser.Name.GivenName, result.FirstName);
        Assert.Equal(scimUser.Name.FamilyName, result.LastName);
        Assert.Equal(scimUser.Emails.FirstOrDefault(), result.Email);
        Assert.Equal(scimUser?.PhoneNumbers?.FirstOrDefault(), result.MobileNumber);
    }

    [Fact]
    public void OrigoUser_To_ScimUser_ProfileTest()
    {
        // Arrange
        var origoUser = GetOrigoUser();
        
        // Act
        var result = _mapper.Map<ScimUser>(origoUser);
        
        // Assert
        Assert.Equal(origoUser.Id.ToString(), result.ExternalId);
        Assert.Equal(origoUser.FirstName, result.Name.GivenName);
        Assert.Equal(origoUser.FirstName, result.Name.GivenName);
        Assert.Equal(origoUser.DisplayName, result.DisplayName);
        Assert.Equal(origoUser.Email, result.Emails.FirstOrDefault());
        Assert.Equal(origoUser.MobileNumber, result?.PhoneNumbers?.FirstOrDefault());
    }
    
    [Fact]
    public void OrigoUser_To_Resource_ProfileTest()
    {
        // Arrange
        var origoUser = GetOrigoUser();
        
        // Act
        var result = _mapper.Map<Resource>(origoUser);
        
        // Assert
        Assert.Equal(origoUser.Id.ToString(), result.Id);
        Assert.Equal(origoUser.Id.ToString(), result.ExternalId);
        Assert.Equal("User", result.Meta.ResourceType);
    }
    
    [Fact]
    public void UserDTO_To_Resource_ProfileTest()
    {
        // Arrange
        var userDto = GetUserDto();

        // Act
        var result = _mapper.Map<Resource>(userDto);

        // Assert
        Assert.Equal(userDto.Id.ToString(), result.Id);
        Assert.Equal(userDto.Id.ToString(), result.ExternalId);
        Assert.Equal("User", result.Meta.ResourceType);
    }

    [Fact]
    public void UserDto_To_User_ProfileTest()
    {
        // Arrange
        var userDto = GetUserDto();

        // Act
        var result = _mapper.Map<User>(userDto);

        // Assert
        Assert.Equal(userDto.Id.ToString(), result.ExternalId);
        Assert.Equal(userDto.LastName, result.Name.FamilyName);
        Assert.Equal(userDto.FirstName, result.Name.GivenName);
        Assert.NotEmpty(result.DisplayName);
        Assert.Equal(userDto.Email, result?.Emails?.FirstOrDefault()?.Value);
        Assert.Equal(userDto.MobileNumber, result?.PhoneNumbers?.FirstOrDefault()?.Value);
        Assert.Equal("User", result?.Meta.ResourceType);
    }
}