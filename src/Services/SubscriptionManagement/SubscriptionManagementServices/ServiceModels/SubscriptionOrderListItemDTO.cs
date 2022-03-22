namespace SubscriptionManagementServices.ServiceModels
{
    public class SubscriptionOrderListItemDTO
    {
        public DateTime CreatedDate { get; set; }
        public string OrderType { get; set; }
        public string PhoneNumber { get; set; }
        public string NewSubscriptionOrderOwnerName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public Guid CreatedBy { get; set; }
        public string OrderNumber { get; set; }
    }
}
