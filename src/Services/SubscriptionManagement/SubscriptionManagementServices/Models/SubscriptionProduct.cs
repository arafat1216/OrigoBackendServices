using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionProduct : Entity
    {
        public SubscriptionProduct()
        {

        }
        public SubscriptionProduct(string subscriptionName, Operator operatorType, IList<Datapackage>? dataPackages, IList<SubscriptionOrder>? subscriptions)
        {

            SubscriptionName = subscriptionName;
            Operator = operatorType;
            DataPackages = dataPackages;
            SubscriptionOrders = subscriptions;

        }

        public string SubscriptionName { get; set; }
        public virtual Operator Operator { get; set; }
        public int OperatorId { get; set; }
        public virtual ICollection<Datapackage>? DataPackages { get; set; }
        public virtual ICollection<SubscriptionOrder>? SubscriptionOrders { get; set; }  
    }
}
