namespace SubscriptionManagement.API.ViewModels
{
    public class UpdatedSubscriptionProduct
    {
        public string SubscriptionName { get; set; }
        public IList<string> Datapackages { get; set; }
    }
}
