
namespace SubscriptionManagementServices.ServiceModels
{
    public class NewSubscriptionOrderDTO
    {
        public NewSubscriptionOrderDTO()
        {
        }

        public NewSubscriptionOrderDTO(NewSubscriptionOrderDTO DTO)
        {
            OperatorId = DTO.OperatorId;
            OperatorName = DTO.OperatorName;
            OperatorAccountId = DTO.OperatorAccountId;
            NewOperatorAccount = DTO.NewOperatorAccount;
            SubscriptionProductName = DTO.SubscriptionProductName;
            DataPackage = DTO.DataPackage;
            OrderExecutionDate = DTO.OrderExecutionDate;
            AddOnProducts = DTO.AddOnProducts;
            SimCardNumber = DTO.SimCardNumber;
            SimCardAction = DTO.SimCardAction;
            SimCardAddress = DTO.SimCardAddress;
            CustomerReferenceFields = DTO.CustomerReferenceFields;
            PrivateSubscription = DTO.PrivateSubscription;
            BusinessSubscription = DTO.BusinessSubscription;
            OperatorAccountPhoneNumber = DTO.OperatorAccountPhoneNumber;
        }

        public int OperatorId { get; set; }
        public string OperatorName { get; set; }

        public int? OperatorAccountId { get; set; }
        public string? OperatorAccountPhoneNumber { get; set; }

        public NewOperatorAccountRequestedDTO? NewOperatorAccount { get; set; }

        public string SubscriptionProductName { get; set; }


        public string? DataPackage { get; set; }

        public DateTime OrderExecutionDate { get; set; }

        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public string? SimCardNumber { get; set; }


        public string SimCardAction { get; set; }

        public SimCardAddressRequestDTO? SimCardAddress { get; set; } = null;

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();

        public PrivateSubscriptionResponse? PrivateSubscription { get; set; } = null;
        public BusinessSubscriptionResponse? BusinessSubscription { get; set; } = null;
    }
}
