using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public record OrigoCancelSubscriptionOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public DateTime DateOfTermination { get; set; }
        public Guid OrganizationId { get; set; }
    }
}
