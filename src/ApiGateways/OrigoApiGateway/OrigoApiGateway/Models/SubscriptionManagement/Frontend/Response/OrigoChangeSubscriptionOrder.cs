using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public record OrigoChangeSubscriptionOrder
    {
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string ProductName { get; set; }
        public string? PackageName { get; set; }
        public string? SubscriptionOwner { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
