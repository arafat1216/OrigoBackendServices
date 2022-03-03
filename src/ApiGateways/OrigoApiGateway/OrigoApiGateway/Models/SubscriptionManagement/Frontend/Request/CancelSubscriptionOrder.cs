using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class CancelSubscriptionOrder
    {
        public string MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public DateTime DateOfTermination { get; set; }
    }
}
