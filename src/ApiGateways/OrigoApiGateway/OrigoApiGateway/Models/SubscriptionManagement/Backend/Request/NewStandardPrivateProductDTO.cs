using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public class NewStandardPrivateProductDTO
    {
        public int OperatorId { get; set; }
        public string SubscriptionName { get; set; }
        public string? DataPackage { get; set; }
        public Guid CallerId { get; set; }
    }
}
