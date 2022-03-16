using SubscriptionManagementServices.Utilities;
using SubscriptionManagementServices.Models;
using Xunit;
using SubscriptionManagementServices.ServiceModels;
using System.Collections.Generic;
using System;

namespace SubscriptionManagement.UnitTests
{
    public class FlatDictionaryTests
    {
        [Fact]
        public void FlatDictionary()
        {
            var flactDictionary = new FlatDictionary();

            var privateSubscription = new PrivateSubscription()
            {
                RealOwner = new PrivateSubscription
                {
                    PostalPlace = "Postal"
                },
                PostalPlace = "Ptal"
            };

            var result1 = flactDictionary.Execute(privateSubscription);
            Assert.Equal("Postal", result1["RealOwner.PostalPlace"]);
            Assert.Equal("Ptal", result1["PostalPlace"]);
            Assert.Equal("", result1["FirstName"]);

            var transferToBusiness = new TransferToBusinessSubscriptionOrder
            {
                PrivateSubscription = new PrivateSubscription
                {
                    RealOwner = new PrivateSubscription
                    {
                        FirstName = "FirstName"
                    }
                },
                BusinessSubscription = new BusinessSubscription
                {
                    Address = "Address"
                },
                CustomerReferenceFields = "{'Key1': 'Val1', 'Key2': 'Val2' }"
            };

            var result2 = flactDictionary.Execute(transferToBusiness);
            Assert.Equal("FirstName", result2["PrivateSubscription.RealOwner.FirstName"]);
            Assert.Equal("Address", result2["BusinessSubscription.Address"]);
            Assert.Equal("{'Key1': 'Val1', 'Key2': 'Val2' }", result2["CustomerReferenceFields"]);
        }

        [Fact]
        public void FlayDictionartTransferToBusiness()
        {
            var newTransferToBusinessFromPrivate = new TransferToBusinessSubscriptionOrderDTO
            {
                BusinessSubscription =
                new BusinessSubscriptionDTO
                {
                    Name = "Comp1",
                    Address = "The Road 1",
                    PostalCode = "1459",
                    PostalPlace = "Oslo",
                    Country = "NO",
                    OrganizationNumber = "911111111"
                },
                PrivateSubscription =
                new PrivateSubscriptionDTO
                {
                    OperatorName = "Telia - NO",
                    FirstName = "Ola",
                    LastName = "Nordmann",
                    Address = "Hjemmeveien 1",
                    PostalCode = "1234",
                    PostalPlace = "HEIMSTADEN",
                    Country = "NO",
                    BirthDate = new DateTime(1971, 10, 21),
                    Email = "me@example.com"
                },
                MobileNumber = "+4791111111",
                SubscriptionProductId = 100,
                DataPackage = "20GB",
                AddOnProducts = new List<string> { "FKL" },
                NewOperatorAccount = new NewOperatorAccountRequestedDTO { NewOperatorAccountOwner = "91700000" },
                CustomerReferenceFields = new List<NewCustomerReferenceValue>
            {
                new() { Name = "URef1", Type = "User", CallerId = Guid.Empty, Value = "VAL1"},
                new() { Name = "URef2", Type = "User", CallerId = Guid.Empty, Value = "VAL2"},
                new() { Name = "AccURef1", Type = "Account", CallerId = Guid.Empty, Value = "VAL3"}
            },
                OperatorAccountId = 10,
                OrderExecutionDate = DateTime.UtcNow.AddDays(7),
                SIMCardAction = "NEW",
                SIMCardNumber = "89722020101228153578",
                CallerId = Guid.Empty
            };

            var flactDictionary = new FlatDictionary();

            var result = flactDictionary.Execute(newTransferToBusinessFromPrivate);
        }
    }
}
