namespace SubscriptionManagementServices.ServiceModels
{
    public class DetailViewSubscriptionOrderLog
    {
        public TransferToBusinessSubscriptionOrderDTOResponse? TransferToBusiness { get; set; }
        public TransferToPrivateSubscriptionOrderDTOResponse? TransferToPrivate { get; set; }
        public OrderSimSubscriptionOrderDTO? OrderSim { get; set; }
        public ActivateSimOrderDTO? ActivateSim { get; set; }
        public NewSubscriptionOrderDTO? NewSubscription { get; set; }
        public ChangeSubscriptionOrderDTO? ChangeSubscription { get; set; }
        public CancelSubscriptionOrderDTO? CancelSubscription { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

    }
}
