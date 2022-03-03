namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public record ChangeSubscriptionOrder
    {
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string ProductName { get; set; }
        public string? PackageName { get; set; }
        public string? SubscriptionOwner { get; set; }
    }
}
