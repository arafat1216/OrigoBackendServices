namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {
        Task<TransferToBusinessSubscriptionOrder> TransferToBusinessSubscriptionOrderAsync(TransferToBusinessSubscriptionOrder subscriptionOrder);
        Task<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrderAsync(TransferToPrivateSubscriptionOrder subscriptionOrder);
        Task<List<ISubscriptionOrder>> GetAllSubscriptionOrdersForCustomer(Guid organizationId);

        Task<ChangeSubscriptionOrder> AddChangeSubscriptionOrderAsync(ChangeSubscriptionOrder subscriptionOrder);
    }
}
