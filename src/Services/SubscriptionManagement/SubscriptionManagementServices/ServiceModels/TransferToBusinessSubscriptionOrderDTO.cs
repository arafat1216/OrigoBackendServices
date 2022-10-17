using System.ComponentModel.DataAnnotations;

namespace SubscriptionManagementServices.ServiceModels
{
    public class TransferToBusinessSubscriptionOrderDTO
    {
        /// <summary>
        /// The current owner the subscription will be transferred from.
        /// </summary>
        public PrivateSubscriptionDTO? PrivateSubscription { get; set; } = null;
        public BusinessSubscriptionDTO? BusinessSubscription { get; set; } = null;
        /// <summary>
        /// The mobile number to be transferred
        /// </summary>
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        /// <summary>
        /// The operator id they get from the business subscription
        /// </summary>
        public int OperatorId { get; set; }

        /// <summary>
        ///     Customer Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }

        /// <summary>
        ///     Data package name
        /// </summary>
        public string? DataPackage { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        [MaxLength(22)]
        public string? SIMCardNumber { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string SIMCardAction { get; set; }
        /// <summary>
        ///     Order address to recive sim card
        /// </summary>
        public SimCardAddressRequestDTO? SimCardAddress { get; set; } = null;

        /// <summary>
        ///     Date of transfer
        /// </summary>
        public DateTime OrderExecutionDate { get; set; }

        /// <summary>
        /// List of add on products to the subscription
        /// </summary>
        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();
        /// <summary>
        ///     A phone number referencing the billing account
        /// </summary>
        [MaxLength(15)]
        public string? OperatorAccountPhoneNumber { get; set; }
        /// <summary>
        ///     New operator account identifier
        /// </summary>
        public int? OperatorAccountId { get; set; }
        /// <summary>
        ///     New operator account only for Telenor
        /// </summary>
        public NewOperatorAccountRequestedDTO? NewOperatorAccount { get; set; }
        public Guid CallerId { get; set; }
    }
}
