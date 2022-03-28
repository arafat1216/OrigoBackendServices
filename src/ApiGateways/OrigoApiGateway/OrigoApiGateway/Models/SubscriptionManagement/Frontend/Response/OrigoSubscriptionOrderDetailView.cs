using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;

namespace OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response
{
    public class OrigoSubscriptionOrderDetailView
    {
        public OrigoTransferToBusinessSubscriptionOrder? TransferToBusiness { get; set; }
        public TransferToPrivateSubscriptionOrder? TransferToPrivate { get; set; }
        public OrigoOrderSim? OrderSim { get; set; }
        public OrigoActivateSimOrder? ActivateSim { get; set; }
        public NewSubscriptionOrder? NewSubscription { get; set; }
        public OrigoChangeSubscriptionOrder? ChangeSubscription { get; set; }
        public OrigoCancelSubscriptionOrder? CancelSubscription { get; set; }
    }
}
