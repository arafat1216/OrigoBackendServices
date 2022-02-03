using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System;

namespace SubscriptionManagement.UnitTests
{
    public class SubscriptionManagementServiceBaseTests
    {
        protected readonly Guid CUSTOMER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD64");
        protected readonly Guid ORGANIZATION_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD64");
        protected readonly Guid CALLER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD64");
        protected SubscriptionManagementServiceBaseTests(DbContextOptions<SubscriptionManagementContext> contextOptions)
        {
            ContextOptions = contextOptions;
            Seed();
        }
        protected DbContextOptions<SubscriptionManagementContext> ContextOptions { get; }

        private void Seed()
        {
            using var context = new SubscriptionManagementContext(ContextOptions);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //Add operator
            var operatorOne = new Operator("Op1", "No", CALLER_ONE_ID);
            context.Add(operatorOne);
            context.SaveChanges();

            //Add customer operator acount
            context.AddRange(
                new CustomerOperatorAccount(ORGANIZATION_ONE_ID, CUSTOMER_ONE_ID, "AC_NUM1", "AC_NAME1", operatorOne.Id, CALLER_ONE_ID),
                new CustomerOperatorAccount(ORGANIZATION_ONE_ID, CUSTOMER_ONE_ID, "AC_NUM2", "AC_NAME2", operatorOne.Id, CALLER_ONE_ID),
                new CustomerOperatorAccount(ORGANIZATION_ONE_ID, CUSTOMER_ONE_ID, "AC_NUM3", "AC_NAME3", operatorOne.Id, CALLER_ONE_ID));

            context.SaveChanges();
        }
    }
}
