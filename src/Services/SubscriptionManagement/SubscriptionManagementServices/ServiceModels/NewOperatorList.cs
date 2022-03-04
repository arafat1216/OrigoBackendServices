namespace SubscriptionManagementServices.ServiceModels
{
    public record NewOperatorList
    {
        public List<int> Operators { get; set; }
        public Guid CallerId { get; set; }
    }
}
