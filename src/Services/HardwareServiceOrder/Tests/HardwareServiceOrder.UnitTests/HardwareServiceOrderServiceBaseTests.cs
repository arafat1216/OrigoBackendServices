﻿using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceBaseTests
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");
        protected readonly Guid CUSTOMER_TWO_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
        protected readonly Guid CUSTOMER_THREE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD64");
        protected readonly Guid CUSTOMER_FOUR_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD65");
        protected readonly Guid CALLER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");

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
            var serviceProvider = new ServiceProvider { OrganizationId = CUSTOMER_ONE_ID };

            var order1 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_ONE_ID, new ContactDetails(CUSTOMER_ONE_ID, "test@test.com", "UserName"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, new ServiceStatus { Id = 100 });
            var order2 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_TWO_ID, new ContactDetails(Guid.NewGuid(), "test@test.com", "UserName"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, new ServiceStatus { Id = 200 }, DateTime.Today.AddDays(-7));
            var order3 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CUSTOMER_THREE_ID, new ContactDetails(Guid.NewGuid(), "test@test.com", "UserName"), Guid.NewGuid(), deliveryAddress, "UserDescription", serviceProvider, Guid.NewGuid().ToString(), null, "externalLink", serviceType, new ServiceStatus { Id = 300 }, DateTime.Today.AddDays(-8));
            var order4 = new HardwareServiceOrderServices.Models.HardwareServiceOrder(CALLER_ONE_ID, CUSTOMER_FOUR_ID, new Guid(), "", new ContactDetails(CUSTOMER_ONE_ID, "test@test.com", "UserName"), deliveryAddress, 1, 1, 1, "serviceProviderId1", "serviceProviderId2", "externalLink", new List<ServiceEvent>());

            context.Add(order1);
            context.Add(order2);
            context.Add(order3);
            context.Add(order4);

            context.SaveChanges();
        }
    }
}
