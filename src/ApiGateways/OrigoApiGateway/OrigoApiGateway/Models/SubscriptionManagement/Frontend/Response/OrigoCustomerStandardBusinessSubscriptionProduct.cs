namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoCustomerStandardBusinessSubscriptionProduct
    {
        public string OperatorName { get; init; }
        public int OperatorId { get; init; }
        public string SubscriptionName { get; init; } 
        public string DataPackage { get; init; }
        public IList<string> AddOnProducts { get; init; }
    }
}
