
namespace SubscriptionManagementServices.ServiceModels
{
    public class NewSubscriptionOrderRequestDTO
    {
        public int OperatorId { get; set; }

        public int? OperatorAccountId { get; set; }
        
        public string? OperatorAccountPhoneNumber { get; set; }
        public NewOperatorAccountRequestedDTO? NewOperatorAccount { get; set; }

        public int SubscriptionProductId { get; set; }


        public string? DataPackage { get; set; }

        public DateTime OrderExecutionDate { get; set; }

        public IList<string> AddOnProducts { get; set; } = new List<string>();

        public string? SimCardNumber { get; set; }


        public string SimCardAction { get; set; }

        public SimCardAddressRequestDTO? SimCardAddress { get; set; } = null;

        public IList<NewCustomerReferenceValue> CustomerReferenceFields { get; set; } = new List<NewCustomerReferenceValue>();

        public PrivateSubscriptionDTO? PrivateSubscription { get; set; } = null;
        public BusinessSubscriptionDTO? BusinessSubscription { get; set; } = null;
        
        public Guid CallerId { get; set; }
    }
}
