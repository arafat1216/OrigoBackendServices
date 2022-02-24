namespace SubscriptionManagementServices.ServiceModels
{
    public class PrivateToBusinessSubscriptionOrderDTO
    {
        /// <summary>
        /// The current owner the subscription will be transferred from.
        /// </summary>
        public PrivateSubscriptionDTO TransferFromPrivateSubscription { get; set; } = new PrivateSubscriptionDTO();

        /// <summary>
        /// The mobile number to be transferred
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        ///     New operator account identifier
        /// </summary>
        public int OperatorAccountId { get; set; }

        /// <summary>
        ///     Subscription product identifier
        /// </summary>
        public int SubscriptionProductId { get; set; }

        /// <summary>
        ///     Data package name
        /// </summary>
        public string DataPackage { get; set; }

        /// <summary>
        ///     Customer identifier
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     SIM card number
        /// </summary>
        public string SIMCardNumber { get; set; }

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
        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public IList<NewCustomerReferenceField> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceField>();

        public CustomerOperatorAccountDTO OperatorAccount { get; set; }
        public Guid CallerId { get; set; }
    }
}
