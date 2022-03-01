using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class TransferToPrivateSubscriptionOrder
    {
        public PrivateSubscription PrivateSubscription { get; set; }
        public string MobileNumber { get; set; }
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
    }
}
