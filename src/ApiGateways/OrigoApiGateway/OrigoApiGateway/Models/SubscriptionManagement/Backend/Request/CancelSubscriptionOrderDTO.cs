using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Backend.Request
{
    public class CancelSubscriptionOrderDTO
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public DateTime DateOfTermination { get; set; }
        public Guid CallerId { get; set; }
    }
}
