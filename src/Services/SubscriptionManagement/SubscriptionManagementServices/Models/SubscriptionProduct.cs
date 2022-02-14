using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionProduct : Entity
    {
        public SubscriptionProduct()
        {

        }
        public SubscriptionProduct(string subscriptionName, int operatorId, IReadOnlyCollection<DataPackage>? dataPackages, Guid callerId)
        {

            SubscriptionName = subscriptionName;
            OperatorId = operatorId;
            DataPackages = dataPackages;
            CreatedBy = callerId;
            UpdatedBy = callerId;
        }

        public string SubscriptionName { get; set; }
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }
        public virtual IReadOnlyCollection<DataPackage>? DataPackages { get; set; }
        public virtual IReadOnlyCollection<SubscriptionOrder>? SubscriptionOrders { get; set; }
    }
}
