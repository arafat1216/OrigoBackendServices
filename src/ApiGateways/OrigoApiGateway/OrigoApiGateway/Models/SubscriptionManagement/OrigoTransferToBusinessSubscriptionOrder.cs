using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement
{
    public record OrigoTransferToBusinessSubscriptionOrder
    {
        /// <summary>
        /// The current owner the subscription will be transferred from.
        /// </summary>
        public OrigoPrivateSubscription? PrivateSubscription { get; set; }
        public OrigoBusinessSubscription? BusinessSubscription { get; set; }
        /// <summary>
        /// The mobile number to be transferred
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        ///     New operator account id
        /// </summary>
        public int? OperatorId { get; set; }
        /// <summary>
        ///     New operator account name
        /// </summary>
        public string? OperatorName { get; set; }

        /// <summary>
        ///     Customer Subscription product name
        /// </summary>
        public string? SubscriptionProductName { get; set; }

        /// <summary>
        ///     Data package name
        /// </summary>
        public string DataPackage { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string? SIMCardNumber { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string SIMCardAction { get; set; }
        /// <summary>
        ///     SIM card reciver address
        /// </summary>
        public SimCardAddress? SimCardAddress { get; set; } = null;


        /// <summary>
        ///     Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }

        /// <summary>
        /// List of add on products to the subscription
        /// </summary>
        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();
        public int? OperatorAccountId { get; set; }
        public string? OperatorAccountName { get; set; }
        public string? OperatorAccountNumber { get; set; }
        public string? OperatorAccountPhoneNumber { get; set; }
        public OrigoNewOperatorAccount? NewOperatorAccount { get; set; }
    }
}
