using SubscriptionManagementServices.Utilities;
using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SubscriptionManagement.UnitTests
{
    public class FlatDictionaryTests
    {
        [Fact]
        public void FlatDictionary()
        {
            var sut = new FlatDictionary();

            var PrivateSubscription = new PrivateSubscription()
            {
                RealOwner = new PrivateSubscription
                {
                    PostalPlace = "Postal"
                },
                PostalPlace = "Ptal"
            };

            var result1 = sut.Execute(PrivateSubscription);

            var transferToBusiness = new TransferToBusinessSubscriptionOrder
            {
                PrivateSubscription = new PrivateSubscription { },
                BusinessSubscription = new BusinessSubscription { },
            };

            var result2 = sut.Execute(transferToBusiness);
        }
    }
}
