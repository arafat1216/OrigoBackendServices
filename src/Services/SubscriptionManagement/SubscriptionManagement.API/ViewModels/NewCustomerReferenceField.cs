namespace SubscriptionManagement.API.ViewModels
{
    public record NewCustomerReferenceField
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public Guid CallerId { get; set; }
    }
}
