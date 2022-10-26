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
        protected readonly string CONNECTED_ORGANIZATION_NUMBER = "11111111111111";
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
            var operatorOne = new Operator(10, "Op1", "No");
            var operatorTwo = new Operator(11, "Op2", "No");
            var operatorThree = new Operator(12, "Op3", "No");
            context.AddRange(operatorOne, operatorTwo, operatorThree);
            context.SaveChanges();

            // Add subscription products
            var dataPackageOne = new DataPackage("Data Package", CALLER_ONE_ID);
            var dataPackageTwo = new DataPackage("Data Package2", CALLER_ONE_ID);
            var dataPackages = new List<DataPackage>();
            dataPackages.Add(dataPackageOne);
            dataPackages.Add(dataPackageTwo);
            context.AddRange(dataPackageOne, dataPackageTwo);
            context.SaveChanges();

            var subscriptionProductOne = new SubscriptionProduct("SubscriptionName", operatorOne, dataPackages, CALLER_ONE_ID);
            context.AddRange(subscriptionProductOne);
            var customerSubscriptionProductOne = new CustomerSubscriptionProduct("SubscriptionName", operatorOne, CALLER_ONE_ID, new List<DataPackage>{ dataPackageOne });
            context.AddRange(customerSubscriptionProductOne);

            //Add customer operator account
            var customerOperatorAccounts = new List<CustomerOperatorAccount>();
            var customerOperatorAccountOne = new CustomerOperatorAccount(1, ORGANIZATION_ONE_ID, CONNECTED_ORGANIZATION_NUMBER, "AC_NUM1", "AC_NAME1", operatorOne.Id, CALLER_ONE_ID);
            var customerOperatorAccountTwo = new CustomerOperatorAccount(2, ORGANIZATION_ONE_ID, CONNECTED_ORGANIZATION_NUMBER, "AC_NUM2", "AC_NAME2", operatorTwo.Id, CALLER_ONE_ID);
            var customerOperatorAccountThree = new CustomerOperatorAccount(3, ORGANIZATION_ONE_ID, CONNECTED_ORGANIZATION_NUMBER, "AC_NUM3", "AC_NAME3", operatorThree.Id, CALLER_ONE_ID);
            customerOperatorAccounts.Add(customerOperatorAccountOne);
            customerOperatorAccounts.Add(customerOperatorAccountTwo);
            customerOperatorAccounts.Add(customerOperatorAccountThree);

            context.AddRange(customerOperatorAccountOne, customerOperatorAccountTwo, customerOperatorAccountThree);
            var customerOperatorSettings = new List<CustomerOperatorSettings>();
           
            var customerOperatorSettingOne = new CustomerOperatorSettings(operatorOne, new List<CustomerSubscriptionProduct>{customerSubscriptionProductOne}, customerOperatorAccounts, null);
            //Setup for operatorsettings two
            var customerOperatorAccountsForOperatorTwo = new List<CustomerOperatorAccount>();
            var customerOperatorAccountForOperatorTwo = new CustomerOperatorAccount(4, ORGANIZATION_ONE_ID, "", "AC_NUM4", "AC_NAME4", operatorTwo.Id, CALLER_ONE_ID);
            customerOperatorAccountsForOperatorTwo.Add(customerOperatorAccountForOperatorTwo);
            context.Add(customerOperatorAccountForOperatorTwo);

            var subscriptionProductTwo = new SubscriptionProduct("Sub2", operatorTwo, null, CALLER_ONE_ID);
            context.AddRange(subscriptionProductTwo);
            var customerSubscriptionProductTwo = new CustomerSubscriptionProduct("Sub2", operatorTwo, CALLER_ONE_ID, null);

            var globalSubscriptionProduct = new CustomerSubscriptionProduct(new SubscriptionProduct("Bedrift +", operatorTwo, null, CALLER_ONE_ID), CALLER_ONE_ID, new List<DataPackage> { new DataPackage("2 GB", CALLER_ONE_ID) });
            context.AddRange(customerSubscriptionProductTwo, globalSubscriptionProduct);

            var customerOperatorSettingTwo = new CustomerOperatorSettings(operatorTwo, new List<CustomerSubscriptionProduct> { customerSubscriptionProductTwo, globalSubscriptionProduct  }, customerOperatorAccountsForOperatorTwo, null);
            
            customerOperatorSettings.Add(customerOperatorSettingOne);
            customerOperatorSettings.Add(customerOperatorSettingTwo);
            context.Add(customerOperatorSettingOne);

            var customerReferences = new List<CustomerReferenceField>();
            var customerReferenceFieldOne = new CustomerReferenceField("UserRef1",SubscriptionManagementServices.Types.CustomerReferenceTypes.User,CALLER_ONE_ID);
            customerReferences.Add(customerReferenceFieldOne);

            context.Add(customerReferenceFieldOne);

            context.AddRange(
               new CustomerSettings(ORGANIZATION_ONE_ID, customerOperatorSettings, customerReferences)
              );

            context.SaveChanges();
        }
    }
}
