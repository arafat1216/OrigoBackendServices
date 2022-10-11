using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Common.Configuration;
using Common.Infrastructure;
using Common.Logging;
using CustomerServices.Email;
using CustomerServices.Exceptions;
using CustomerServices.Infrastructure;
using CustomerServices.Infrastructure.Context;
using CustomerServices.ServiceModels;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace CustomerServices.UnitTests;

public class PartnerServicesTests
{
    private readonly OrganizationServices _organizationServices;
    private readonly PartnerServices _partnerServices;
    private DbContextOptions<CustomerContext> ContextOptions { get; }

    public PartnerServicesTests()
    {
        ContextOptions = new DbContextOptionsBuilder<CustomerContext>()
            .UseSqlite($"Data Source={Guid.NewGuid()}.db").Options;
        new UnitTestDatabaseSeeder().SeedUnitTestDatabase(ContextOptions);
        var mappingConfig = new MapperConfiguration(mc => { mc.AddMaps(Assembly.GetAssembly(typeof(LocationDTO))); });
        var mapper = mappingConfig.CreateMapper();
        var apiRequesterServiceMock = new Mock<IApiRequesterService>();
        apiRequesterServiceMock.Setup(r => r.AuthenticatedUserId).Returns(UnitTestDatabaseSeeder.CALLER_ID);
        var context = new CustomerContext(ContextOptions, apiRequesterServiceMock.Object);
        var organizationRepository =
            new OrganizationRepository(context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
        _organizationServices =
            new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), organizationRepository, mapper, Mock.Of<IEmailService>(), 
            Options.Create(new TechstepPartnerConfiguration
            {
               PartnerId = UnitTestDatabaseSeeder.TECHSTEP_PARTNER_ID
            }));
        _partnerServices = new PartnerServices(Mock.Of<ILogger<PartnerServices>>(), organizationRepository);
    }

    [Fact]
    public async Task CreatePartnerAsyncTest1()
    {
        var organization = await _organizationServices.GetOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        organization!.ChangePartner(null);
        organization = await _organizationServices.UpdateOrganizationAsync(organization);

        var partner = await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);

        // Make sure ID is generated correctly
        Assert.True(partner.ExternalId != Guid.Empty);

        // Make sure the timestamps are +-30 minutes of the current UTC time.
        // If these tests fail, then UTC has not been used or there are problems with setting the timestamp correctly.
        Assert.True(DateTime.UtcNow < partner.CreatedDate.AddMinutes(30));
        Assert.True(DateTime.UtcNow.AddMinutes(30) > partner.CreatedDate);
        Assert.True(DateTime.UtcNow < partner.LastUpdatedDate.AddMinutes(15));
        Assert.True(DateTime.UtcNow.AddMinutes(30) > partner.LastUpdatedDate);

        // Check that the caller IDs are correct
        Assert.Equal(UnitTestDatabaseSeeder.CALLER_ID, partner.CreatedBy);
        Assert.Equal(Guid.Empty, partner.UpdatedBy);

        // Make sure we have retrieved the correct organization (we don't test for all values as we only attach the retrieved entity).
        Assert.Equal(organization.Id, partner.Organization.Id);
        Assert.Equal(organization.OrganizationId, partner.Organization.OrganizationId);

        // Misc
        Assert.False(partner.IsDeleted);
    }

    // Negative test to ensure we cant create a partner from non-existing organizations
    [Fact]
    public async Task CreatePartnerAsyncTest2()
    {
        await Assert.ThrowsAsync<CustomerNotFoundException>(async () =>
            await _partnerServices.CreatePartnerAsync(Guid.Parse("00000000-0000-0000-0000-000000000002")));
    }

    // Negative test that makes sure we can't register a single organization as a two partners (prevents accidental creation duplicates)
    [Fact]
    public async Task CreatePartnerAsyncTest3()
    {
        // Ensure the organization don't have a partner or customer status
        var organization = await _organizationServices.GetOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        organization!.ChangePartner(null);
        await _organizationServices.UpdateOrganizationAsync(organization);

        // Register the 1st partner instance, and then unset the organization's auto-assigned partner so we can recycle it
        var partner = await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        partner.Organization.ChangePartner(null);
        await _organizationServices.UpdateOrganizationAsync(partner.Organization);

        await Assert.ThrowsAsync<DuplicateException>(async () =>
            await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID));
    }

    // Negative test that ensures we can't assign a customer as a partner.
    [Fact]
    public async Task CreatePartnerAsyncTest4()
    {
        // Ensure the organization don't have a partner or customer status
        var organization = await _organizationServices.GetOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        organization!.ChangePartner(null);
        await _organizationServices.UpdateOrganizationAsync(organization);

        // Create the partner, and remove the assigned partner it's organization so we can re-use it
        var partner = await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        partner.Organization.ChangePartner(partner);
        await _organizationServices.UpdateOrganizationAsync(partner.Organization);

        await Assert.ThrowsAsync<ArgumentException>(async () =>
            await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID));
    }

    [Fact]
    public async Task GetPartnerAsyncTest()
    {
        // Ensure the organization don't have a partner or customer status
        var organization = await _organizationServices.GetOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        organization!.ChangePartner(null);
        await _organizationServices.UpdateOrganizationAsync(organization);

        var partner = await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        var result1 =
            await _partnerServices.GetPartnerAsync(
                Guid.Parse("00000000-0000-0000-0000-000000000002"), true); // Partner not found (null)
        var result2 = await _partnerServices.GetPartnerAsync(partner.ExternalId, true); // Partner found

        // Make sure a non-existing partner returns null.
        Assert.True(result1 == null);

        // Make sure the partner exists and have correct external ID.
        Assert.True(result2 != null);
        Assert.Equal(partner.ExternalId, result2?.ExternalId);
    }

    [Fact]
    public async Task GetPartnersAsyncTest()
    {
        // Ensure the organization don't have a partner or customer status
        var organization = await _organizationServices.GetOrganizationAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        organization!.ChangePartner(null);
        await _organizationServices.UpdateOrganizationAsync(organization);

        
        var partnerList = await _partnerServices.GetPartnersAsync();

        //Partner that exists is not the Customer One
        Assert.All(partnerList, item => Assert.NotEqual(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, item.Organization.OrganizationId));
        Assert.True(partnerList is not null);
        Assert.True(partnerList.Count == 2);

        // List with results
        await _partnerServices.CreatePartnerAsync(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID);
        partnerList = await _partnerServices.GetPartnersAsync();

        Assert.True(partnerList is not null);
        Assert.True(partnerList?.Count == 3);

        Assert.Collection(partnerList, item => Assert.Equal(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, item.Organization.OrganizationId),
                                  item => Assert.NotEqual(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, item.Organization.OrganizationId),
                                  item => Assert.NotEqual(UnitTestDatabaseSeeder.CUSTOMER_ONE_ID, item.Organization.OrganizationId));
    }

    [Fact]
    public async Task GetPartners_IsSortedOnName()
    {
        // Act
        var partners = await _partnerServices.GetPartnersAsync();

        // Assert
        Assert.Equal("PARTNER", partners.ElementAt(0).Organization.Name);
        Assert.Equal("TECHSTEP", partners.ElementAt(1).Organization.Name);
    }
}