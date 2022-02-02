namespace SubscriptionManagement.API.ViewModels
{
    public class SubscriptionOrder
    {
        public int SubscriptionProductId { get; set; }
        public Guid OrganizationId { get; set; }
        public int OperatorAccountId { get; set; }
        public int DatapackageId { get; set; }
    }
}
