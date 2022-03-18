namespace SubscriptionManagementServices.ServiceModels
{
    public class NewCustomerStandardPrivateSubscriptionProduct
    {
        public int OperatorId { get; set; }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
        public Guid CallerId { get; set; }
    }
}
