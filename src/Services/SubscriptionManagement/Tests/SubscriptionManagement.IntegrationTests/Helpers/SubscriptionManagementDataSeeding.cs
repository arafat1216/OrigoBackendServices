
using SubscriptionManagementServices.Infrastructure;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SubscriptionManagement.IntegrationTests.Helpers
{
    internal static class SubscriptionManagementDataSeeding
    {
        public static int FIRST_OPERATOR_ID;
        public static readonly int CUSTOMER_SUBSCRIPTION_PRODUCT_ID = 200;
        public static readonly int SUBSCRIPTION_PRODUCT_ID = 300;
        public static readonly int OPERATOR_ACCOUNT_ID = 100;
        public static readonly Guid ORGANIZATION_ID = Guid.Parse("7adbd9fa-97d1-11ec-8500-00155d64bd3d");
        public static readonly Guid ORGANIZATION_TWO_ID = Guid.Parse("86764171-8706-425e-ace7-2845c91e6161");
        public static readonly Guid CALLER_ID = Guid.NewGuid();
        public static readonly string PHONE_NUMBER = "99999998";

        public static async void PopulateData(SubscriptionManagementContext subscriptionManagementContext)
        {
            subscriptionManagementContext.Database.EnsureCreated();
            var firstOperator = subscriptionManagementContext.Operators.FirstOrDefault();
            FIRST_OPERATOR_ID = firstOperator!.Id;
            var subscriptionProduct = new SubscriptionProduct(SUBSCRIPTION_PRODUCT_ID, "TOTAL BEDRIFT", firstOperator!, new List<DataPackage> { new DataPackage("20GB", Guid.Empty), new DataPackage("30GB", Guid.Empty) }, Guid.Empty);
            subscriptionManagementContext.Add(subscriptionProduct);

            var customerSubscriptionProduct = new CustomerSubscriptionProduct(CUSTOMER_SUBSCRIPTION_PRODUCT_ID, subscriptionProduct, Guid.Empty, (IList<DataPackage>?)subscriptionProduct.DataPackages);
            var customerOperatorAccount = new CustomerOperatorAccount(OPERATOR_ACCOUNT_ID, ORGANIZATION_ID, "1111111111111", "435543", "CC1", firstOperator!.Id, Guid.Empty);
            subscriptionManagementContext.CustomerOperatorAccounts.Add(customerOperatorAccount);

            var standardPrivateProduct = new CustomerStandardPrivateSubscriptionProduct("PrivateDataPackage", "PrivateSubscription", Guid.Empty);
            subscriptionManagementContext.CustomerStandardPrivateSubscriptionProducts.Add(standardPrivateProduct);

            var standardBusinessProduct = new CustomerStandardBusinessSubscriptionProduct("BusinessDataPackage", "BusinessSubscription", Guid.Empty, new List<SubscriptionAddOnProduct> { new SubscriptionAddOnProduct("Faktura kontroll", Guid.NewGuid()) });
            subscriptionManagementContext.CustomerStandardBusinessSubscriptionProduct.Add(standardBusinessProduct);
            var customerOperatorSettings = new List<CustomerOperatorSettings>
                {
                    new (firstOperator,
                        new List<CustomerSubscriptionProduct> { customerSubscriptionProduct },
                        new List<CustomerOperatorAccount> { customerOperatorAccount },
                        standardPrivateProduct,
                        standardBusinessProduct
                        )
                };
            var customerReferenceFields = new List<CustomerReferenceField>
                {
                    new CustomerReferenceField("URef1", CustomerReferenceTypes.User, Guid.Empty),
                    new CustomerReferenceField("URef2", CustomerReferenceTypes.User, Guid.Empty),
                    new CustomerReferenceField("AccURef1", CustomerReferenceTypes.Account, Guid.Empty)
                };
            subscriptionManagementContext.CustomerSettings.Add(new CustomerSettings(ORGANIZATION_ID,
                customerOperatorSettings, customerReferenceFields));

            subscriptionManagementContext.TransferToPrivateSubscriptionOrders.Add(new TransferToPrivateSubscriptionOrder()
            {
                UserInfo = new PrivateSubscription(
                    "EndUser", "Test", "office address", "1219", "Oslo", "NO", "test@techstep.no", DateTime.UtcNow,
                    "Telia", null
                    ),
                MobileNumber = PHONE_NUMBER,
                OperatorName = "Telia",
                NewSubscription = "New",
                OrderExecutionDate = DateTime.UtcNow,
                OrganizationId = ORGANIZATION_ID,
                SalesforceTicketId = "911"
            });

            //Customer two
            var telenorNorwayOperator = subscriptionManagementContext.Operators.FirstOrDefault(o => o.OperatorName == "Telenor - NO");
            var telenorSubscription = subscriptionManagementContext.SubscriptionProducts.FirstOrDefault(o => o.Operator.OperatorName == "Telenor - NO" && o.SubscriptionName.Contains("Bedrift"));
            var operatorSettingsCustomerTwo = new CustomerOperatorSettings(telenorNorwayOperator, null, CALLER_ID);
            subscriptionManagementContext.Add(operatorSettingsCustomerTwo);
            var customerSubscriptionProductCustomerTwo = new CustomerSubscriptionProduct(telenorSubscription, CALLER_ID, null);
            var customerSettingsCustomerTwo = new CustomerSettings(ORGANIZATION_TWO_ID, new List<CustomerOperatorSettings>{ operatorSettingsCustomerTwo}, null);
            subscriptionManagementContext.Add(customerSettingsCustomerTwo);



            subscriptionManagementContext.SaveChanges();
        }

    }
}
