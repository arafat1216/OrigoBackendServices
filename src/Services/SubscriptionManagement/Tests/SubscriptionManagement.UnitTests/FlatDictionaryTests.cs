using SubscriptionManagementServices.Utilities;
using SubscriptionManagementServices.Models;
using Xunit;

namespace SubscriptionManagement.UnitTests
{
    public class FlatDictionaryTests
    {
        [Fact]
        public void FlatDictionary()
        {
            var sut = new FlatDictionary();

            var privateSubscription = new PrivateSubscription()
            {
                RealOwner = new PrivateSubscription
                {
                    PostalPlace = "Postal"
                },
                PostalPlace = "Ptal"
            };

            var result1 = sut.Execute(privateSubscription);
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

            var result2 = sut.Execute(transferToBusiness);
            Assert.Equal("FirstName", result2["PrivateSubscription.RealOwner.FirstName"]);
            Assert.Equal("Address", result2["BusinessSubscription.Address"]);
            Assert.Equal("{'Key1': 'Val1', 'Key2': 'Val2' }", result2["CustomerReferenceFields"]);
        }
    }
}
