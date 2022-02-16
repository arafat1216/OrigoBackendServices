using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagement.API.ViewModels
{
    public record CustomerSubscriptionProduct
    {
        public CustomerSubscriptionProduct(string subscriptionName)
        {
            SubscriptionName = subscriptionName;
        }
        public int Id { get; set; }
        public string SubscriptionName { get; set; }
        public OperatorDTO Operator { get; set; }
        public IList<string> Datapackages { get; set; }
        public bool isGlobal { get; set; }
    }
}
