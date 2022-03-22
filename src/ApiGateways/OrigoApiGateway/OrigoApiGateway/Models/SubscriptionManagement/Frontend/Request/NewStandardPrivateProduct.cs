namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class NewStandardPrivateProduct
    {
        public int OperatorId { get; set; }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
    }
}
