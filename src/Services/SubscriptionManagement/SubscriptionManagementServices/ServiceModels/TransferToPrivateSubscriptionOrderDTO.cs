namespace SubscriptionManagementServices.ServiceModels

namespace SubscriptionManagementServices.ServiceModels
{
    public class TransferToPrivateSubscriptionOrderDTO
    {
        public TransferToPrivateSubscriptionOrderDTO()
        {
        }

        public TransferToPrivateSubscriptionOrderDTO(TransferToPrivateSubscriptionOrderDTO DTO)
        {
            PrivateSubscription = DTO.PrivateSubscription;
            MobileNumber = DTO.MobileNumber;
            OperatorId = DTO.OperatorId;
            OperatorName = DTO.OperatorName;
            NewSubscription = DTO.NewSubscription;
            OrderExecutionDate = DTO.OrderExecutionDate;
            CallerId = DTO.CallerId;
        }

        public PrivateSubscriptionResponse PrivateSubscription { get; set; }
        [Phone]
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid CallerId { get; set; }
    }
}
