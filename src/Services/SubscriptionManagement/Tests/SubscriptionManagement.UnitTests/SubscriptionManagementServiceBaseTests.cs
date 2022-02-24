using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;

namespace SubscriptionManagement.UnitTests
{
    public class SubscriptionManagementServiceBaseTests
    {
        
        protected readonly Guid ORGANIZATION_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD62");
        protected readonly Guid CALLER_ONE_ID = new("42447F76-D9A8-4F0A-B0FF-B4683ACEDD63");
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
            var operatorTwo = new Operator("Op2", "No", CALLER_ONE_ID);
            var operatorThree = new Operator("Op3", "No", CALLER_ONE_ID);
            context.AddRange(operatorOne, operatorTwo, operatorThree);
            context.SaveChanges();
            

            //Add customer operator acount
            var customerOperatorAccounts = new List<CustomerOperatorAccount>();
            var customerOperatorAccountOne = new CustomerOperatorAccount(ORGANIZATION_ONE_ID, "AC_NUM1", "AC_NAME1", operatorOne.Id, CALLER_ONE_ID);
            var customerOperatorAccountTwo = new CustomerOperatorAccount(ORGANIZATION_ONE_ID, "AC_NUM2", "AC_NAME2", operatorTwo.Id, CALLER_ONE_ID);
            var customerOperatorAccountThree = new CustomerOperatorAccount(ORGANIZATION_ONE_ID, "AC_NUM3", "AC_NAME3", operatorThree.Id, CALLER_ONE_ID);
            customerOperatorAccounts.Add(customerOperatorAccountOne);
            customerOperatorAccounts.Add(customerOperatorAccountTwo);
            customerOperatorAccounts.Add(customerOperatorAccountThree);

            context.AddRange(customerOperatorAccountOne, customerOperatorAccountTwo, customerOperatorAccountThree);
            var customerOperatorSettings = new List<CustomerOperatorSettings>();
           
            var customerOperatorSettingOne = new CustomerOperatorSettings(operatorOne, customerOperatorAccounts, CALLER_ONE_ID);
            customerOperatorSettings.Add(customerOperatorSettingOne);
            context.Add(customerOperatorSettingOne);

            context.AddRange(
               new CustomerSettings(ORGANIZATION_ONE_ID, customerOperatorSettings)
              );

            context.SaveChanges();

            var dataPackageOne = new DataPackage("Data Package", CALLER_ONE_ID);
            var dataPackageTwo = new DataPackage("Data Package2", CALLER_ONE_ID);
            var dataPackages = new List<DataPackage>();
            dataPackages.Add(dataPackageOne);
            dataPackages.Add(dataPackageTwo);

            context.SaveChanges();

            context.AddRange(dataPackageOne,dataPackageTwo);

            var subscriptionProductOne = new SubscriptionProduct("SubscriptionName", operatorOne, dataPackages, CALLER_ONE_ID);

            context.AddRange(subscriptionProductOne);

            var customerSubscriptionProductOne = new CustomerSubscriptionProduct("SubscriptionName", operatorOne, CALLER_ONE_ID, null);

            context.AddRange(customerSubscriptionProductOne);

            context.SaveChanges();
        }
    }
}
