﻿using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class NewSubscriptionOrder
    {
        /// <summary>
        /// The operator to be transferred to 
        /// </summary>
        public int OperatorId { get; set; }
        /// <summary>
        ///     Operator account identifier
        /// </summary>
        public int? OperatorAccountId { get; set; }
        /// <summary>
        ///     Reference for account
        /// </summary>
        [MaxLength(15)]
        public string? OperatorAccountPhoneNumber { get; set; }

        /// <summary>
        ///     New operator account identifier
        /// </summary>
        public NewOperatorAccountRequested? NewOperatorAccount { get; set; }

        /// <summary>
        ///     Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }

        /// <summary>
        ///     Data package name
        /// </summary>
        [MaxLength(50)]
        public string? DataPackage { get; set; }
        /// <summary>
        ///     Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }
        /// <summary>
        /// List of add on products to the subscription
        /// </summary>
        public List<string> AddOnProducts { get; set; } = new List<string>();
        /// <summary>
        ///     Sim card number
        /// </summary>
        [MaxLength(22)]
        public string? SimCardNumber { get; set; }

        /// <summary>
        ///     Sim card number
        /// </summary>
        public string SimCardAction { get; set; }
        /// <summary>
        ///     Billing address the sim card should be sent to
        /// </summary>
        public SimCardAddress? SimCardAddress { get; set; } = null;
        /// <summary>
        /// Customer reference field
        /// </summary>
        public List<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new();
        /// <summary>
        /// User information
        /// </summary>
        public PrivateSubscription? PrivateSubscription { get; set; } = null;
        /// <summary>
        ///  Company information if non-personal subscription
        /// </summary>
        public BusinessSubscription? BusinessSubscription { get; set; } = null;

    }
}
