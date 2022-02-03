using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionOrder : Entity
    {
        public SubscriptionOrder()
        {

        }
        public SubscriptionOrder(int subscriptionProductId, Guid organizationId, int operatorAccountId, int datapackageId, Guid callerId)
        {
            OrganizationId = organizationId;
            SubscriptionProductId = subscriptionProductId;
            OperatorAccountId = operatorAccountId;
            DatapackageId = datapackageId;
            CreatedBy = callerId;
            UpdatedBy = callerId;   
        }
        
        public virtual SubscriptionProduct SubscriptionProduct { get; set; }
        public int SubscriptionProductId { get; set; }
        public Guid OrganizationId { get; set; }
        public virtual CustomerOperatorAccount OperatorAccount { get; set; }
        public int OperatorAccountId { get; set; }
        public virtual Datapackage? DataPackage { get; set; }
        public int DatapackageId { get; set; }
        public virtual ICollection<SubscriptionAddOnProduct> SubscriptionAddOnProducts { get; set; }
    }
}
