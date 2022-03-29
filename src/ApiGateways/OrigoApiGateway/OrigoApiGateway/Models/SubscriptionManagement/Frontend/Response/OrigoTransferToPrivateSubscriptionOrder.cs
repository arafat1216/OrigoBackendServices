using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoTransferToPrivateSubscriptionOrder
    {
        public OrigoPrivateSubscription PrivateSubscription { get; set; }
        public string? MobileNumber { get; set; }
        public int OperatorId { get; set; }
        public string? OperatorName { get; set; }
        public string? NewSubscriptionName { get; set; }
        public DateTime OrderExecutionDate { get; set; }
    }
}
