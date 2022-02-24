using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionOrder : Entity, ISubscriptionOrder
    {
        private List<SubscriptionAddOnProduct> _subscriptionAddOnProducts;

        public SubscriptionOrder()
        {
            _subscriptionAddOnProducts = new List<SubscriptionAddOnProduct>();
        }

        public SubscriptionOrder(
            string simCardNumber,
            string simCardAction,
            int subscriptionProductId,
            Guid oranizationId,
            int operatorAccountId,
            int dataPackageId,
            DateTime orderExecutionDate,
            string mobileNumber,
            string customerReferenceFields,
            List<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            SimCardNumber = simCardNumber;
            SIMCardAction = simCardAction;
            SubscriptionProductId = subscriptionProductId;
            OrganizationId = oranizationId;
            OperatorAccountId = operatorAccountId;
            DataPackageId = dataPackageId;
            OrderExecutionDate = orderExecutionDate;
            MobileNumber = mobileNumber;
            CustomerReferenceFields = customerReferenceFields;
            _subscriptionAddOnProducts = subscriptionAddOnProducts;
        }

        public CustomerSubscriptionProduct CustomerSubscriptionProduct { get; set; }
        public IReadOnlyCollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts => _subscriptionAddOnProducts.AsReadOnly();
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

        public void  SetSubscriptionAddOnProduct(List<SubscriptionAddOnProduct> subscriptionAddOnProducts)
        {
            _subscriptionAddOnProducts.AddRange(subscriptionAddOnProducts);
        }
    }
}
