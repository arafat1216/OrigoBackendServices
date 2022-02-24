namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionOrder
    {
        public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }
        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; }
        public string SimCardNumber { get; set; }
        public string SIMCardAction { get; set; }
        public int SubscriptionProductId { get; set; }
        public Guid OrganizationId { get; set; }
        public CustomerOperatorAccount? OperatorAccount { get; set; }
        public int? OperatorAccountId { get; set; }
        public DataPackage? DataPackage { get; set; }
        public int DataPackageId { get; set; }
        public DateTime OrderExecutionDate { get; set; }
        public string MobileNumber { get; set; }
        public string CustomerReferenceFields { get; set; }

        void SetSubscriptionAddOnProduct(List<SubscriptionAddOnProduct> subscriptionAddOnProducts);
    }
}
