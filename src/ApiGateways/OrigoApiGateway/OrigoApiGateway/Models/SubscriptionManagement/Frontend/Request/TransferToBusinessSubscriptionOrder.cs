using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class TransferToBusinessSubscriptionOrder
    {
        /// <summary>
        /// The current owner the subscription will be transferred from.
        /// </summary>
        public PrivateSubscription? PrivateSubscription { get; set; } = null;
        public BusinessSubscription? BusinessSubscription { get; set; } = null;

        /// <summary>
        /// The mobile number to be transferred
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        ///     New operator account identifier
        /// </summary>
        public int? OperatorAccountId { get; set; }

        /// <summary>
        ///     Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }

        /// <summary>
        ///     Data package name
        /// </summary>
        public string? DataPackage { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string? SIMCardNumber { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string SIMCardAction { get; set; }

        /// <summary>
        ///     Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }

        /// <summary>
        /// List of add on products to the subscription
        /// </summary>
        public List<string> AddOnProducts { get; set; } = new List<string>();

        public List<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new();
        public NewOperatorAccountRequested? NewOperatorAccount { get; set; }
    }
}