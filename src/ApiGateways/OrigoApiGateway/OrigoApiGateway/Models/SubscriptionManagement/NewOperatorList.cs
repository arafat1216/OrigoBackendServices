using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class NewOperatorList
    {
        /// <summary>
        /// A list of avalible operators for customer
        /// </summary>
        public IList<string> Operators{ get; set; }
    }
}
