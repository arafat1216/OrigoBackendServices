namespace SubscriptionManagementServices.ServiceModels
{
    public class DetailViewSubscriptionOrderLog
    {
        public TransferToBusinessSubscriptionOrderDTOResponse? TransferToBusiness { get; set; }
        public TransferToPrivateSubscriptionOrderDTOResponse? TransferToPrivate { get; set; }
        public OrderSimSubscriptionOrderDTO? OrderSim { get; set; }
        public ActivateSimOrderDTOResponse? ActivateSim { get; set; }
        public NewSubscriptionOrderDTO? NewSubscription { get; set; }
        public ChangeSubscriptionOrderDTO? ChangeSubscription { get; set; }
        public CancelSubscriptionOrderDTOResponse? CancelSubscription { get; set; }

    }
}
