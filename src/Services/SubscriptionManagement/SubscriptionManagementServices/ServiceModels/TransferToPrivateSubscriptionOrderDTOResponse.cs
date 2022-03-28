namespace SubscriptionManagementServices.ServiceModels
{
    public class TransferToPrivateSubscriptionOrderDTOResponse
    {
        public TransferToPrivateSubscriptionOrderDTOResponse()
        {
        }

        public TransferToPrivateSubscriptionOrderDTOResponse(TransferToPrivateSubscriptionOrderDTOResponse DTO)
        {
            PrivateSubscription = DTO.PrivateSubscription;
            MobileNumber = DTO.MobileNumber;
            OperatorId = DTO.OperatorId;
            OperatorName = DTO.OperatorName;
            NewSubscriptionName = DTO.NewSubscriptionName;
            OrderExecutionDate = DTO.OrderExecutionDate;
            
        }

        public PrivateSubscriptionResponse PrivateSubscription { get; set; }
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscriptionName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
    }
}
