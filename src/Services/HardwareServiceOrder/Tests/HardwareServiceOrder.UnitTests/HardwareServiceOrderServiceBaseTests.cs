using HardwareServiceOrderServices.Infrastructure;
using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HardwareServiceOrder.UnitTests
{
    public class HardwareServiceOrderServiceBaseTests
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");
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

            var deliveryAddress = new DeliveryAddress("Recipient", "Description", "Address1", "Address2", "PostalCode", "City", "Country");
            var serviceTye = new ServiceType() {  };
            var serviceStatus = new ServiceStatus() { };
            var serviceProvider = new ServiceProvider { OrganizationId = CUSTOMER_ONE_ID };
            
            //context.Add(deliveryAddress);
            //context.Add(serviceTye);
            //context.Add(serviceStatus);
            //context.Add(serviceProvider);
            //context.SaveChanges();

            var user = new User(Guid.NewGuid(), "test@test.com", "UserName");
            //var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(assetLifecycleId: Guid.NewGuid(), deliveryAddress, "UserDescription", "externalLink", serviceTye, serviceStatus, serviceProvider);
            var order = new HardwareServiceOrderServices.Models.HardwareServiceOrder(assetLifecycleId: Guid.NewGuid(), deliveryAddress, "UserDescription", "externalLink", serviceTye, serviceStatus, serviceProvider, user);
            context.Add(order);
            
            context.SaveChanges();
        }
    }
}
