using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using System;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoSubscriptionOrderDetailView
    {
        public OrigoTransferToBusinessSubscriptionOrder? TransferToBusiness { get; set; }
        public OrigoTransferToPrivateSubscriptionOrder? TransferToPrivate { get; set; }
        public OrigoOrderSim? OrderSim { get; set; }
        public OrigoActivateSimOrder? ActivateSim { get; set; }
        public OrigoNewSubscriptionOrder? NewSubscription { get; set; }
        public OrigoChangeSubscriptionOrder? ChangeSubscription { get; set; }
        public OrigoCancelSubscriptionOrder? CancelSubscription { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
