using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request
{
    public class TransferToPrivateSubscriptionOrder
    {
        public PrivateSubscription PrivateSubscription { get; set; }
        [MaxLength(15)]
        public string MobileNumber { get; set; }
        [MaxLength(50)]
        public string OperatorName { get; set; }
        public string NewSubscription { get; set; }
        public DateTime OrderExecutionDate { get; set; }
    }
}
