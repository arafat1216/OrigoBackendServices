using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionOrder : Entity
    {
        public SubscriptionOrder()
        {

        }
        public SubscriptionOrder(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId)
        {
            CustomerId = customerId;
            SubscriptionProductId = subscriptionProductId;
            OperatorAccountId = operatorAccountId;
            DatapackageId = datapackageId;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public virtual SubscriptionProduct SubscriptionProduct { get; set; }
        public int SubscriptionProductId { get; set; }
        public Guid CustomerId { get; set; }
        public virtual CustomerOperatorAccount OperatorAccount { get; set; }
        public int OperatorAccountId { get; set; }
        public virtual Datapackage? DataPackage { get; set; }
        public int DatapackageId { get; set; }
        public virtual ICollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
    }
}
