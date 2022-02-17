using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class NewSubscriptionProduct
    {
        /// <summary>
        /// The product name
        /// </summary>
        public string SubscriptionProductName { get; set; }

        /// <summary>
        /// The operator name
        /// </summary>
        public string OperatorName { get; set; }

        /// <summary>
        /// The datapacakges that is offered by the product
        /// </summary>
        public IList<string> DataPackages { get; set; }


    }
}
