namespace SubscriptionManagementServices.ServiceModels
{
    public class TransferToPrivateSubscriptionOrderDTO
    {
        public PrivateSubscriptionDTO PrivateSubscription { get; set; }
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
