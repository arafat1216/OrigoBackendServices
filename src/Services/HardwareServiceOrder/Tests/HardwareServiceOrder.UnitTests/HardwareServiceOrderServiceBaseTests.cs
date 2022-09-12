using HardwareServiceOrderServices.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceBaseTests
    {
        protected readonly Guid CUSTOMER_ONE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");
        protected readonly Guid CUSTOMER_TWO_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
        protected readonly Guid CUSTOMER_THREE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD64");
        protected readonly Guid CUSTOMER_FOUR_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD65");
        protected readonly Guid CALLER_ONE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
        protected readonly int CUSTOMER_SERVICE_PROVIDER_ONE = (int)ServiceProviderEnum.ConmodoNo;

        protected HardwareServiceOrderServiceBaseTests(DbContextOptions<HardwareServiceOrderContext> dbContext)
        {
            ContextOptions = dbContext;
            Seed();
        }

        protected DbContextOptions<HardwareServiceOrderContext> ContextOptions { get; }

        private void Seed()
        {
            // This test-ID may eventually need to be changed should it start to conflict with the seed data.
            int serviceProviderId = 700;

            using var context = new HardwareServiceOrderContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            DeliveryAddress deliveryAddress = new(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");
            ServiceType serviceType = new() { Id = 400 };
            List<ServiceProviderServiceType> serviceProviderServiceTypes = new()
            {
                new(serviceProviderId, (int)ServiceTypeEnum.SUR)
            };
            List<ServiceOrderAddon> serviceProviderServiceAddons = new()
            {
                // These test-IDs may eventually need to be changed should it start to conflict with the seed data.
                new ServiceOrderAddon(701, serviceProviderId, "[ThirdPartyId1]", false, false, Guid.NewGuid(), DateTimeOffset.Parse("2020-01-21")),
                new ServiceOrderAddon(702, serviceProviderId, "[ThirdPartyId2]", false, true, Guid.NewGuid(), DateTimeOffset.Parse("2020-02-22")),
                new ServiceOrderAddon(703, serviceProviderId, "[ThirdPartyId3]", true, false, Guid.NewGuid(), DateTimeOffset.Parse("2020-03-23")),
                new ServiceOrderAddon(704, serviceProviderId, "[ThirdPartyId4]", true, true, Guid.NewGuid(), DateTimeOffset.Parse("2020-04-24")),
            };

            ServiceProvider serviceProvider = new(serviceProviderId, "[ServiceProviderName]", CUSTOMER_ONE_ID, true, true, serviceProviderServiceTypes, serviceProviderServiceAddons, CUSTOMER_ONE_ID, DateTimeOffset.Parse("2020-10-15"));
            AssetInfo assetInfo = new("[AssetBrand]", "[AssetModel]", new HashSet<string>() { "527127734377463" }, "[SerialNumber]", DateOnly.Parse("2020-01-01"), null);

            var order1 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_ONE_ID, Guid.NewGuid(), 1, assetInfo, "UserDescription", new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), deliveryAddress, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, null, "serviceProviderOrderId1", null, "externalLink", new List<ServiceEvent>());
            var order2 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_TWO_ID, new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, "serviceProviderOrderId1", null, "externalLink", serviceType, new ServiceStatus { Id = 200 }, DateTime.Today.AddDays(-7));
            var order3 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_THREE_ID, new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, "serviceProviderOrderId1", null, "externalLink", serviceType, new ServiceStatus { Id = 300 }, DateTime.Today.AddDays(-8));
            var order4 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_FOUR_ID, Guid.NewGuid(), 1, assetInfo, "UserDescription", new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), deliveryAddress, (int)ServiceTypeEnum.Recycle, (int)ServiceStatusEnum.Unknown, (int)ServiceProviderEnum.ConmodoNo, null, "serviceProviderOrderId1", "serviceProviderOrderId2", "externalLink", new List<ServiceEvent>());

            var cmServiceProvider1 = new CustomerServiceProvider
            {
                CustomerId = CUSTOMER_ONE_ID,
                ApiPassword = "",
                ApiUserName = "",
                ServiceProviderId = CUSTOMER_SERVICE_PROVIDER_ONE,
                LastUpdateFetched = DateTime.Today,
            };
            var cmServiceProvider2 = new CustomerServiceProvider
            {
                CustomerId = CUSTOMER_TWO_ID,
                ApiPassword = "",
                ApiUserName = "",
                ServiceProviderId = CUSTOMER_SERVICE_PROVIDER_ONE,
                LastUpdateFetched = DateTime.Today.AddDays(-1),
            };
            
            var cmServiceProvider3 = new CustomerServiceProvider
            {
                CustomerId = CUSTOMER_THREE_ID,
                ApiPassword = "",
                ApiUserName = "",
                ServiceProviderId = CUSTOMER_SERVICE_PROVIDER_ONE,
                LastUpdateFetched = DateTime.Today.AddDays(-1),
            };

            context.Add(order1);
            context.Add(order2);
            context.Add(order3);
            context.Add(order4);
            context.AddRange(cmServiceProvider1, cmServiceProvider2, cmServiceProvider3);

            context.SaveChanges();

            var apiCredentials1 = new ApiCredential(cmServiceProvider1.Id, (int)ServiceTypeEnum.SUR, "", "");
            var apiCredentials2 = new ApiCredential(cmServiceProvider1.Id, (int)ServiceTypeEnum.Remarketing, "", "");
            var apiCredentials3 = new ApiCredential(cmServiceProvider2.Id, (int)ServiceTypeEnum.SUR, "", "");
            var apiCredentials4 = new ApiCredential(cmServiceProvider2.Id, (int)ServiceTypeEnum.Remarketing, "", "");
            var apiCredentials5 = new ApiCredential(cmServiceProvider3.Id, null, "", "");

            context.AddRange(apiCredentials1, apiCredentials2, apiCredentials3, apiCredentials4, apiCredentials5);

            context.SaveChanges();
        }
    }
}
