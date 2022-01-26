using Common.Seedwork;

namespace SubscriptionManagementServices.Models
{
    public class SubscriptionProduct : Entity
    {
        public SubscriptionProduct(string subscriptionName, Operator operatorType, IList<Datapackage>? dataPackages, IList<SubscriptionOrder>? subscriptions)
        {
           
            SubscriptionName = subscriptionName;
            OperatorType = operatorType;
            DataPackages = dataPackages;
            
        }
        public string SubscriptionName { get; set; }
        public Operator OperatorType { get; set; }
        public IList<Datapackage>? DataPackages { get; set; }
    }
}
