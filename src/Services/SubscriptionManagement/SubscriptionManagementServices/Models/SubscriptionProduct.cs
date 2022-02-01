using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionProduct : Entity
    {
        public SubscriptionProduct()
        {

        }
        public SubscriptionProduct(string subscriptionName, int operatorId)
        {

            SubscriptionName = subscriptionName;
            OperatorId = operatorId;
        }

        public string SubscriptionName { get; set; }
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }
        public virtual ICollection<Datapackage>? DataPackages { get; set; }
        public virtual ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }  
    }
}
