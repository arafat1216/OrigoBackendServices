using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public class OrderTransferSubscription
    {
        /// <summary>
        /// The operatorId
        /// </summary>
        public int OperatorId { get; set; }
        /// <summary>
        /// The operator account number
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// Name of subscription
        /// </summary>
        public string SubscriptionName { get; set; }
        /// <summary>
        /// Datapackage with subscription
        /// </summary>
        public string? Datapackage { get; set; }
        /// <summary>
        /// Organization it is linked to
        /// </summary>
        public Guid OrganizationId { get; set; }
        /// <summary>
        /// List of add on products to the subscription
        /// </summary>
        public IList<string> AddOnProducts { get; set; }
    }
}
