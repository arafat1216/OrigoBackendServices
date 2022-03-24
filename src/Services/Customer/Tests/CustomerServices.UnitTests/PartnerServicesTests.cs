using Xunit;
using System;
using System.Threading.Tasks;
using CustomerServices.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using CustomerServices.UnitTests;
using Common.Logging;
using CustomerServices.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using CustomerServices.Exceptions;

namespace CustomerServices.Tests
{
    public class PartnerServicesTests : OrganizationServicesBaseTest
    {
        private readonly CustomerContext _context;
        private readonly OrganizationServices _organizationServices;
        private readonly PartnerServices _partnerServices;

        private Guid CallerId { get; init; }

        public PartnerServicesTests() : base(new DbContextOptionsBuilder<CustomerContext>().UseSqlite("Data Source=sqlitepartnerunittests.db").Options)
        {
            _context = new CustomerContext(ContextOptions);
            var _organizationRepository = new OrganizationRepository(_context, Mock.Of<IFunctionalEventLogService>(), Mock.Of<IMediator>());
            _organizationServices = new OrganizationServices(Mock.Of<ILogger<OrganizationServices>>(), _organizationRepository);
            _partnerServices = new PartnerServices(Mock.Of<ILogger<PartnerServices>>(), _organizationRepository);

            CallerId = Guid.NewGuid();
        }


        [Fact()]
        public async Task CreatePartnerAsyncTest1()
        {
            var organization = await _organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);
            organization.ChangePartner(null, CallerId);
            organization = await _organizationServices.UpdateOrganizationAsync(organization);

            var partner = await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId);

            // Make sure ID is generated correctly
            Assert.True(partner.ExternalId != Guid.Empty);

            // Make sure the timestamps are +-30 minutes of the current UTC time.
            // If these tests fail, then UTC has not been used or there are problems with setting the timestamp correctly.
            Assert.True(DateTime.UtcNow < partner.CreatedDate.AddMinutes(30));
            Assert.True(DateTime.UtcNow.AddMinutes(30) > partner.CreatedDate);
            Assert.True(DateTime.UtcNow < partner.LastUpdatedDate.AddMinutes(15));
            Assert.True(DateTime.UtcNow.AddMinutes(30) > partner.LastUpdatedDate);

            // Check that the caller IDs are correct
            Assert.Equal(CallerId, partner.CreatedBy);
            Assert.Equal(CallerId, partner.UpdatedBy);

            // Make sure we have retrieved the correct organization (we dont test for all values as we only attach the retrieved entity).
            Assert.Equal(organization.Id, partner.Organization.Id);
            Assert.Equal(organization.OrganizationId, partner.Organization.OrganizationId);

            // Misc
            Assert.False(partner.IsDeleted);
        }

        // Negative test to ensure we cant create a partner from non-existing organizations
        [Fact()]
        public async Task CreatePartnerAsyncTest2()
        {
            await Assert.ThrowsAsync<CustomerNotFoundException>(async () =>
                await _partnerServices.CreatePartnerAsync(Guid.Parse("00000000-0000-0000-0000-000000000002"), CallerId)
            );
        }

        // Negative test that makes sure we can't register a single organization as a two partners (prevents accidental creation duplicates)
        [Fact()]
        public async Task CreatePartnerAsyncTest3()
        {
            // Ensure the organization don't have a partner or customer status
            var organization = await _organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);
            organization.ChangePartner(null, CallerId);
            organization = await _organizationServices.UpdateOrganizationAsync(organization);

            // Register the 1st partner instance, and then unset the organization's auto-assigned partner so we can recycle it
            var partner = await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId);
            partner.Organization.ChangePartner(null, CallerId);
            await _organizationServices.UpdateOrganizationAsync(partner.Organization);

            await Assert.ThrowsAsync<DuplicateException>(async () =>
                await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId)
            );
        }

        // Negative test that ensures we can't assign a customer as a partner.
        [Fact()]
        public async Task CreatePartnerAsyncTest4()
        {
            // Ensure the organization don't have a partner or customer status
            var organization = await _organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);
            organization.ChangePartner(null, CallerId);
            organization = await _organizationServices.UpdateOrganizationAsync(organization);

            // Create the partner, and remove the assigned partner it's organization so we can re-use it
            var partner = await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId);
            partner.Organization.ChangePartner(partner, CallerId);
            await _organizationServices.UpdateOrganizationAsync(partner.Organization);

            await Assert.ThrowsAsync<ArgumentException>(async () =>
                await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId)
            );
        }

        [Fact()]
        public async Task GetPartnerAsyncTest()
        {
            // Ensure the organization don't have a partner or customer status
            var organization = await _organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);
            organization.ChangePartner(null, CallerId);
            organization = await _organizationServices.UpdateOrganizationAsync(organization);

            var partner = await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId);
            var result1 = await _partnerServices.GetPartnerAsync(Guid.Parse("00000000-0000-0000-0000-000000000002")); // Partner not found (null)
            var result2 = await _partnerServices.GetPartnerAsync(partner.ExternalId); // Partner found

            // Make sure a non-existing partner returns null.
            Assert.True(result1 == null);

            // Make sure the partner exists and have correct external ID.
            Assert.True(result2 != null);
            Assert.Equal(partner.ExternalId, result2?.ExternalId);
        }

        [Fact()]
        public async Task GetPartnersAsyncTest()
        {
            // Ensure the organization don't have a partner or customer status
            var organization = await _organizationServices.GetOrganizationAsync(CUSTOMER_ONE_ID);
            organization.ChangePartner(null, CallerId);
            organization = await _organizationServices.UpdateOrganizationAsync(organization);

            // Empty list
            var results = await _partnerServices.GetPartnersAsync();

            Assert.True(results is not null);
            Assert.True(results?.Count == 0);

            // List with results
            await _partnerServices.CreatePartnerAsync(CUSTOMER_ONE_ID, CallerId);
            results = await _partnerServices.GetPartnersAsync();

            Assert.True(results is not null);
            Assert.True(results?.Count == 1);
        }

    }
}