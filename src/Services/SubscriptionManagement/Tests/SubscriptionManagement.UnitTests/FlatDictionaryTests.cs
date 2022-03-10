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

            var res = sut.Execute(PrivateSubscription);
        }
    }
}
