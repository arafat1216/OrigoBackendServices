using Common.Configuration;
using Common.Infrastructure;
using Common.Logging;
using CustomerServices.Email;
using CustomerServices.Email.Models;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace CustomerServices.UnitTests;

public class OrganizationServicesTests
{
    private readonly IMapper _mapper;
    private readonly OrganizationServices _organizationServices;
    private readonly Mock<IEmailService> _emailServiceMock;
    private DbContextOptions<CustomerContext> ContextOptions { get; }


    public OrganizationServicesTests()
    {
        ContextOptions = new DbContextOptionsBuilder<CustomerContext>()
            .UseSqlite($"Data Source={Guid.NewGuid()}.db").Options;
        new UnitTestDatabaseSeeder().SeedUnitTestDatabase(ContextOptions);
        var mappingConfig =
            new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(LocationDTO))); });
        _mapper = mappingConfig.CreateMapper();
        var apiRequesterServiceMock = new Mock<IApiRequesterService>();
        apiRequesterServiceMock.Setup(r => r.AuthenticatedUserId).Returns(UnitTestDatabaseSeeder.CALLER_ID);
        var context = new CustomerContext(ContextOptions, apiRequesterServiceMock.Object);
        var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var emailMock = new Mock<IEmailService>();
        _emailServiceMock = emailMock;
        _organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository, _mapper, emailMock.Object,
            Options.Create(new TechstepPartnerConfiguration
            {
                PartnerId = UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID
            }));
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetCompanyOne_CheckName()
    {
        // Act
        var organization = await _organizationServices.GetOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);

        // Assert
        Assert.Equal("COMPANY ONE", organization!.Name);
        Assert.Equal("My Way 1", organization.Address.Street);
        Assert.Equal("1111", organization.Address.PostCode);
        Assert.Equal("My City", organization.Address.City);
        Assert.Equal("NO", organization.Address.Country);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOrganizationIdsForPartner_CheckReturnedIds()
    {
        // Act
        var organizationIdsForPartner = await _organizationServices.GetOrganizationIdsForPartnerAsync(UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID);

        // Assert
        Assert.Equal(2, organizationIdsForPartner.Count);
        Assert.Equal(UnitTestDatabaseSeeder.TECHSTEP_CUSTOMER_ID, organizationIdsForPartner.First());
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PutCompanyOne_null_values()
    {
        // Arrange
        var orgData = new UpdateOrganizationDTO()
        {
            OrganizationId = UnitTestDatabaseSeeder.CUSTOMER_ONE_ID,
            ParentId = null,
            PrimaryLocation = null,
            CallerId = Guid.Empty,
            Name = "Mytos",
            LastDayForReportingSalaryDeduction = 1
        };

        // Act
        var organization = await _organizationServices.PutOrganizationAsync(orgData);

        // Assert 
        Assert.Equal("Mytos", organization.Name);
        Assert.Equal("", organization.OrganizationNumber);
        Assert.Null(organization.ParentId);
        Assert.True(organization.PrimaryLocation!.IsNull());
        Assert.Equal(UnitTestDatabaseSeeder.CALLER_ID, organization.UpdatedBy);
        Assert.Equal("", organization.Address.Street);
        Assert.Equal("", organization.Address.PostCode);
        Assert.Equal("", organization.Address.City);
        Assert.Equal("", organization.Address.Country);
        Assert.Equal("", organization.ContactPerson.FirstName);
        Assert.Equal("", organization.ContactPerson.LastName);
        Assert.Equal("", organization.ContactPerson.Email);
        Assert.Equal("", organization.ContactPerson.PhoneNumber);
    }


    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOrganization_WithoutAddOktaUsersSet_CheckDefaultValueSetToFalse()
    {
        // Act
        var organization = await _organizationServices.AddOrganizationAsync(new NewOrganizationDTO
        {
            Name = "COMPANY NAME",
            OrganizationNumber = "999999999",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO()
        });

        // Assert 
        Assert.False(organization.AddUsersToOkta);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOrganization_WithAddOktaUsersSet_CheckValue()
    {
        // Act
        var organization = await _organizationServices.AddOrganizationAsync(new NewOrganizationDTO
        {
            Name = "COMPANY NAME",
            OrganizationNumber = "999999999",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO(),
            AddUsersToOkta = true
        });

        // Assert 
        Assert.True(organization.AddUsersToOkta);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOrganization_initializedStatus_BeforOnboarding()
    {
        // Act
        var organization = await _organizationServices.AddOrganizationAsync(new NewOrganizationDTO
        {
            Name = "COMPANY NAME",
            OrganizationNumber = "999999999",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO()
        });

        // Assert 
        Assert.Equal(Common.Enums.CustomerStatus.BeforeOnboarding, organization.Status);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOrganization_WithTechstepAsPartner_ReturnTechstepCustomerIdAndAccountOwner()
    {
        // Act
        var accountOwner = "Krister Emanuelsen";
        var techstepCustomerId = 112223232321;

        var organization = await _organizationServices.AddOrganizationAsync(new NewOrganizationDTO
        {
            Name = "Techstep mini",
            OrganizationNumber = "22222222278",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO(),
            AddUsersToOkta = true,
            PartnerId = UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID,
            AccountOwner = accountOwner,
            TechstepCustomerId = techstepCustomerId
        });

        // Assert 
        Assert.Equal(accountOwner, organization.AccountOwner);
        Assert.Equal(techstepCustomerId, organization.TechstepCustomerId);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOrganization_WithTechstepAsPartner_OnlyTechstepCustomerId()
    {
        //Arrange
        var techstepCustomerId = 112223232321;
        var organization = await _organizationServices.AddOrganizationAsync(new NewOrganizationDTO
        {
            Name = "Techstep mini",
            OrganizationNumber = "9897878787878",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO(),
            AddUsersToOkta = true,
            PartnerId = UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID,
            TechstepCustomerId = techstepCustomerId
        });

        // Assert 
        Assert.Equal("", organization.AccountOwner);
        Assert.Equal(techstepCustomerId, organization.TechstepCustomerId);

    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task AddOrganization_NotTechstepAsPartner_ShouldNotSavetechstepCustomerId()
    {
        //Arrange
        var accountOwner = "Krister Emanuelsen";

        var newOrganization = new NewOrganizationDTO
        {
            Name = "Techstep mini",
            OrganizationNumber = "23233222",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO(),
            AddUsersToOkta = true,
            PartnerId = UnitTestDatabaseSeeder.PARTNER_ID,
            TechstepCustomerId = 122323312312,
            AccountOwner = accountOwner
        };

        //Act and Assert
        var organization = await _organizationServices.AddOrganizationAsync(newOrganization);

        Assert.Null(organization.TechstepCustomerId);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PutCompanyOne_partial_null_values()
    {
        // Arrange
        var orgData = new UpdateOrganizationDTO()
        {
            OrganizationId = UnitTestDatabaseSeeder.CUSTOMER_ONE_ID,
            ParentId = null,
            PrimaryLocation = null,
            CallerId = Guid.Empty,
            Name = "name",
            Address = new AddressDTO()
            {
                Street = "street",
            },
            ContactPerson = new ContactPersonDTO()
            {
                FirstName = "FirstName"
            },
            LastDayForReportingSalaryDeduction = 1
        };

        // Act
        var organization = await _organizationServices.PutOrganizationAsync(orgData);

        // Assert 
        Assert.Equal("name", organization.Name);
        Assert.Equal("", organization.OrganizationNumber);
        Assert.Null(organization.ParentId);
        Assert.True(organization.PrimaryLocation!.IsNull());
        Assert.Equal(UnitTestDatabaseSeeder.CALLER_ID, organization.UpdatedBy);
        Assert.Equal("street", organization.Address.Street);
        Assert.Equal("", organization.Address.PostCode);
        Assert.Equal("", organization.Address.City);
        Assert.Equal("", organization.Address.Country);
        Assert.Equal("FirstName", organization.ContactPerson.FirstName);
        Assert.Equal("", organization.ContactPerson.LastName);
        Assert.Equal("", organization.ContactPerson.Email);
        Assert.Equal("", organization.ContactPerson.PhoneNumber);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PutOrganization_WithAddOktaUsersSet_CheckValue()
    {
        // Arrange
        var orgData = new UpdateOrganizationDTO()
        {
            OrganizationId = UnitTestDatabaseSeeder.CUSTOMER_ONE_ID,
            ParentId = null,
            PrimaryLocation = null,
            CallerId = Guid.Empty,
            Name = "name",
            Address = new AddressDTO()
            {
                Street = "street"
            },
            ContactPerson = new ContactPersonDTO()
            {
                FirstName = "FirstName"
            },
            LastDayForReportingSalaryDeduction = 1,
            AddUsersToOkta = true
        };

        // Act
        var organization = await _organizationServices.PutOrganizationAsync(orgData);

        // Assert 
        Assert.True(organization.AddUsersToOkta);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PatchOrganization_WithAddOktaUsersSet_CheckValue()
    {
        // Act
        var organization = await _organizationServices.PatchOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, null, Guid.Empty, "name", null, "street", null, null, null, "FirstName", null, null, null, 1, "", addUsersToOkta: true);

        // Assert 
        Assert.True(organization.AddUsersToOkta);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOrganization_WithAddOktaUsersSet_CheckValue()
    {
        // Arrange
        var organization = await _organizationServices.AddOrganizationAsync(new NewOrganizationDTO
        {
            Name = "COMPANY NAME",
            OrganizationNumber = "999999999",
            Location = new LocationDTO(),
            Address = new AddressDTO(),
            ContactPerson = new ContactPersonDTO(),
            IsCustomer = true,
            AddUsersToOkta = true
        });

        // Act
        var addedOrganization = await _organizationServices.GetOrganizationAsync(organization.OrganizationId);

        // Assert 
        Assert.True(addedOrganization!.AddUsersToOkta);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PatchCompanyOne__null_values()
    {
        // Act
        var organization = await _organizationServices.PatchOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, null, Guid.Empty, null, null, null, null, null, null, null, null, null, null, 1, "");

        // Assert 
        Assert.Equal("COMPANY ONE", organization.Name);
        Assert.Equal("999888777", organization.OrganizationNumber);
        Assert.Null(organization.ParentId);
        Assert.NotNull(organization.PrimaryLocation);
        Assert.Equal(Guid.Empty, organization.UpdatedBy);
        Assert.Equal("My Way 1", organization.Address.Street);
        Assert.Equal("1111", organization.Address.PostCode);
        Assert.Equal("My City", organization.Address.City);
        Assert.Equal("NO", organization.Address.Country);
        Assert.Equal("JOHN", organization.ContactPerson.FirstName);
        Assert.Equal("DOE", organization.ContactPerson.LastName);
        Assert.Equal("john.doe@example.com", organization.ContactPerson.Email);
        Assert.Equal("99999999", organization.ContactPerson.PhoneNumber);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PatchCompanyOne_partial_null_values()
    {
        // Act
        var organization = await _organizationServices.PatchOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, null, Guid.Empty, "name", null, "street", null, null, null, null, "Paavola", null, null, 1, "");

        // Assert 
        Assert.Equal("name", organization.Name);
        Assert.Equal("999888777", organization.OrganizationNumber);
        Assert.Null(organization.ParentId);
        Assert.NotNull(organization.PrimaryLocation);
        Assert.Equal(UnitTestDatabaseSeeder.CALLER_ID, organization.UpdatedBy);
        Assert.Equal("street", organization.Address.Street);
        Assert.Equal("1111", organization.Address.PostCode);
        Assert.Equal("My City", organization.Address.City);
        Assert.Equal("NO", organization.Address.Country);
        Assert.Equal("JOHN", organization.ContactPerson.FirstName);
        Assert.Equal("Paavola", organization.ContactPerson.LastName);
        Assert.Equal("john.doe@example.com", organization.ContactPerson.Email);
        Assert.Equal("99999999", organization.ContactPerson.PhoneNumber);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task InitiateOnboardingAsync_CallEmailOnlyOnUserStatusNotInvited()
    {
        // Act
        var organization = await _organizationServices.InitiateOnboardingAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        // Arrange
        _emailServiceMock.Verify(email => email.InvitationEmailToUserAsync(It.IsAny<InvitationMail>(), It.IsAny<string>()), Times.Exactly(2));
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task GetOrganizationByTechstepCustomerIdAsync_ReturnsCustomer()
    {
        // Act
        var context = new CustomerContext(ContextOptions, null);
        var organizationRepository = new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        var organization = await organizationRepository.GetOrganizationByTechstepCustomerIdAsync(123456789);
        // Assert
        Assert.Equal(UnitTestDatabaseSeeder.CUSTOMER_FIVE_ID, organization?.OrganizationId);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PutOrganizationAsync_WithTechstepPartner_NotUpdateNameAndOrgNumb()
    {

        //Arrange
        var organizationUpdate = new UpdateOrganizationDTO
        {
            OrganizationId = UnitTestDatabaseSeeder.CUSTOMER_FOUR_ID,
            OrganizationNumber = "55555555",
            Name = "Hallo på do!",
        };
        // Act
        var organization = await _organizationServices.PutOrganizationAsync(organizationUpdate);
        // Arrange
        Assert.Equal("COMPANY FOUR", organization.Name);
        Assert.Equal("999555444", organization.OrganizationNumber);
    }
    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PutOrganizationAsync_WithNoPartner_UpdateNameAndOrganizationNumb()
    {
        var newOrgNumber = "55555555";
        var newName = "Hallo på do!";
        //Arrange
        var organizationUpdate = new UpdateOrganizationDTO
        {
            OrganizationId = UnitTestDatabaseSeeder.CUSTOMER_ONE_ID,
            OrganizationNumber = newOrgNumber,
            Name = newName,
        };
        // Act
        var organization = await _organizationServices.PutOrganizationAsync(organizationUpdate);
        // Arrange
        Assert.Equal(newName, organization.Name);
        Assert.Equal(newOrgNumber, organization.OrganizationNumber);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PatchOrganizationAsync_WithTechstepPartner_NotUpdateNameAndOrgNumb()
    {

        //Arrange
        var newOrgNumber = "55555555";
        var newName = "Hallo på do!";
        // Act
        var organization = await _organizationServices.PatchOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_FOUR_ID, null, null, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, newName, newOrgNumber, null, null, null, null, null, null, null, null, null);
        // Arrange
        Assert.Equal("COMPANY FOUR", organization.Name);
        Assert.Equal("999555444", organization.OrganizationNumber);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PatchOrganizationAsync_WithNoPartner_UpdateNameAndOrganizationNumb()
    {
        //Arrange
        var newOrgNumber = "55555555";
        var newName = "Hallo på do!";

        // Act
        var organization = await _organizationServices.PatchOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, null, null, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, newName, newOrgNumber, null, null, null, null, null, null, null, null, null);
        // Arrange
        Assert.Equal(newName, organization.Name);
        Assert.Equal(newOrgNumber, organization.OrganizationNumber);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task PatchOrganizationAsync_WithPartnerThatIsNotTechstep_UpdateNameAndOrganizationNumb()
    {
        //Arrange
        var newOrgNumber = "55555555";
        var newName = "Hallo på do!";

        // Act
        var organization = await _organizationServices.PatchOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_THREE_ID, null, null, UnitTestDatabaseSeeder.EMPTY_CALLER_ID, newName, newOrgNumber, null, null, null, null, null, null, null, null, null);
        // Arrange
        Assert.Equal(newName, organization.Name);
        Assert.Equal(newOrgNumber, organization.OrganizationNumber);
    }

    [Fact]
    [Trait("Category", "UnitTest")]
    public async Task SaveHashedApiKey_CheckGetHashedApiKey()
    {
        // Act
        await _organizationServices.GenerateAndSaveHashedApiKeyAsync(UnitTestDatabaseSeeder.CUSTOMER_THREE_ID, CancellationToken.None);
        var hashedApiKey = await _organizationServices.GetHashedApiKeyAsync(UnitTestDatabaseSeeder.CUSTOMER_THREE_ID, CancellationToken.None);

        // Assert
        Assert.False(string.IsNullOrEmpty(hashedApiKey));
        Assert.Matches(@"^([^\:]*\:){3}[^\:]*$", hashedApiKey);
    }
}