

namespace SubscriptionManagementServices.ServiceModels
{
    public class TransferToBusinessSubscriptionOrderDTOResponse
    {
        public TransferToBusinessSubscriptionOrderDTOResponse()
        {
        }

        public TransferToBusinessSubscriptionOrderDTOResponse(TransferToBusinessSubscriptionOrderDTOResponse DTO)
        {
            PrivateSubscription = DTO.PrivateSubscription;
            BusinessSubscription = DTO.BusinessSubscription;
            MobileNumber = DTO.MobileNumber;
            OperatorId = DTO.OperatorId;
            OperatorName = DTO.OperatorName;
            SubscriptionProductName = DTO.SubscriptionProductName;
            DataPackage = DTO.DataPackage;
            SIMCardNumber = DTO.SIMCardNumber;
            SIMCardAction = DTO.SIMCardAction;
            OrderExecutionDate = DTO.OrderExecutionDate;
            AddOnProducts = DTO.AddOnProducts;
            CustomerReferenceFields = DTO.CustomerReferenceFields;
            NewOperatorAccount = DTO.NewOperatorAccount;
            CallerId = DTO.CallerId;
            OperatorAccountPhoneNumber = DTO.OperatorAccountPhoneNumber; 
        }

        /// <summary>
        /// The current owner the subscription will be transferred from.
        /// </summary>
        public PrivateSubscriptionResponse? PrivateSubscription { get; set; } = null;
        public BusinessSubscriptionResponse? BusinessSubscription { get; set; } = null;
        /// <summary>
        /// The mobile number to be transferred
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        ///   New operator id
        /// </summary>
        public int OperatorId { get; set; }
        /// <summary>
        ///     New operator
        /// </summary>
        public string? OperatorName { get; set; }
        

        /// <summary>
        ///     Customer Subscription product name
        /// </summary>
        public string SubscriptionProductName { get; set; }

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
        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();
        public string? OperatorAccountPhoneNumber { get; set; }
        public NewOperatorAccountRequestedDTO? NewOperatorAccount { get; set; }
        public Guid CallerId { get; set; }
    }
}