using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public record ChangeSubscriptionOrderPostRequest
    {
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string ProductName { get; set; }
        public string? PackageName { get; set; }
        public string? SubscriptionOwner { get; set; }
        public Guid CallerId { get; set; }
    }
}
