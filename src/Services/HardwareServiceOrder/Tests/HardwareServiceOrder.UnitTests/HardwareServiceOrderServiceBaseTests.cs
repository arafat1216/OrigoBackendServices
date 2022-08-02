using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceBaseTests
    {
        protected readonly Guid CUSTOMER_ONE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");
        protected readonly Guid CUSTOMER_TWO_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
        protected readonly Guid CUSTOMER_THREE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD64");
        protected readonly Guid CUSTOMER_FOUR_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD65");
        protected readonly Guid CALLER_ONE_ID = Guid.Parse("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");

        protected HardwareServiceOrderServiceBaseTests(DbContextOptions<HardwareServiceOrderContext> dbContext)
        {
            ContextOptions = dbContext;
            Seed();
        }

        protected DbContextOptions<HardwareServiceOrderContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new HardwareServiceOrderContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            var deliveryAddress = new DeliveryAddress(RecipientTypeEnum.Personal, "Recipient", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceType = new ServiceType() { Id = 400 };
            var serviceProvider = new ServiceProvider("ServiceProviderName", CUSTOMER_ONE_ID, new HashSet<ServiceProviderServiceType>());
            AssetInfo assetInfo = new("[AssetBrand]", "[AssetModel]", new HashSet<string>() { "527127734377463" }, "[SerialNumber]", DateOnly.Parse("2020-01-01"), null);

            var order1 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_ONE_ID, Guid.NewGuid(), 1, assetInfo, "UserDescription", new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), deliveryAddress, (int)ServiceTypeEnum.SUR, (int)ServiceStatusEnum.Ongoing, (int)ServiceProviderEnum.ConmodoNo, "serviceProviderOrderId1", null, "externalLink", new List<ServiceEvent>());
            var order2 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_TWO_ID, new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, "serviceProviderOrderId1", null, "externalLink", serviceType, new ServiceStatus { Id = 200 }, DateTime.Today.AddDays(-7));
            var order3 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_THREE_ID, new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, "serviceProviderOrderId1", null, "externalLink", serviceType, new ServiceStatus { Id = 300 }, DateTime.Today.AddDays(-8));
            var order4 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_FOUR_ID, Guid.NewGuid(), 1, assetInfo, "UserDescription", new ContactDetails(CUSTOMER_ONE_ID, "FirstName", "LastName", "test@test.com", "PhoneNumber"), deliveryAddress, (int)ServiceTypeEnum.Recycle, (int)ServiceStatusEnum.Unknown, (int)ServiceProviderEnum.ConmodoNo, "serviceProviderOrderId1", "serviceProviderOrderId2", "externalLink", new List<ServiceEvent>());

            var cmServiceProvider1 = new CustomerServiceProvider
            {
                CustomerId = CUSTOMER_ONE_ID,
                ApiPassword = "",
                ApiUserName = "",
                ServiceProviderId = 1,
                LastUpdateFetched = DateTime.Today,
            };
            var cmServiceProvider2 = new CustomerServiceProvider
            {
                CustomerId = CUSTOMER_TWO_ID,
                ApiPassword = "",
                ApiUserName = "",
                ServiceProviderId = 1,
                LastUpdateFetched = DateTime.Today.AddDays(-1),
            };

            context.Add(order1);
            context.Add(order2);
            context.Add(order3);
            context.Add(order4);
            context.AddRange(cmServiceProvider1, cmServiceProvider2);

            context.SaveChanges();
        }
    }
}
