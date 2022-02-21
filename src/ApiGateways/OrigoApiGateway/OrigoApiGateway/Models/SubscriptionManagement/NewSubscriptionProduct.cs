using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class NewSubscriptionProduct
    {
        /// <summary>
        /// The subscription product name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The operator name
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        /// The datapacakges that is offered by the product
        /// </summary>
        public IList<string> DataPackages { get; set; }


    }
}
