namespace SubscriptionManagement.API.ViewModels
{
    public class ExistingSubscriptionProduct
    {
        public string SubscriptionName { get; set; }
        public IList<string> Datapackages { get; set; }
    }
}
