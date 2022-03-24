
namespace SubscriptionManagementServices.ServiceModels
{
    public class NewSubscriptionOrderDTO
    {
        public NewSubscriptionOrderDTO()
        {
        }

        public NewSubscriptionOrderDTO(NewSubscriptionOrderDTO DTO)
        {
            MobileNumber = DTO.MobileNumber;
            OperatorId = DTO.OperatorId;
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
        }

        public string MobileNumber { get; set; }

        public int OperatorId { get; set; }

        public int? OperatorAccountId { get; set; }

        public NewOperatorAccountRequestedDTO? NewOperatorAccount { get; set; }

        public string SubscriptionProductName { get; set; }


        public string? DataPackage { get; set; }

        public DateTime OrderExecutionDate { get; set; }

        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public string? SimCardNumber { get; set; }


        public string SimCardAction { get; set; }

        public SimCardAddressRequestDTO? SimCardAddress { get; set; } = null;

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();

        public PrivateSubscriptionDTO? PrivateSubscription { get; set; } = null;
        public BusinessSubscriptionDTO? BusinessSubscription { get; set; } = null;
    }
}
