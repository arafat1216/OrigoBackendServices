namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public record ChangeSubscriptionOrder
    {
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        [MaxLength(50)]
        public string OperatorName { get; set; }
        public string ProductName { get; set; }
        public string? PackageName { get; set; }
        public string? SubscriptionOwner { get; set; }
    }
}
