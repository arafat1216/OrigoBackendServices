using Common.Enums;

namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository<T> where T : ISubscriptionOrder
    {
        Task<List<ISubscriptionOrder>> GetAllSubscriptionOrdersForCustomer(Guid organizationId);
        Task<int> GetTotalSubscriptionOrdersCountForCustomer(Guid organizationId, IList<OrderTypes>? orderTypes = null, string? phoneNumber = null);
        Task<T> AddSubscriptionOrder(T subscriptionOrder);
        Task<OrderSimSubscriptionOrder> GetOrderSimOrder(Guid subscriptionOrder);
        Task<ActivateSimOrder> GetActivateSimOrder(Guid subscriptionOrder);
        Task<TransferToBusinessSubscriptionOrder> GetTransferToBusinessOrder(Guid subscriptionOrder);
        Task<TransferToPrivateSubscriptionOrder> GetTransferToPrivateOrder(Guid subscriptionOrder);
        Task<NewSubscriptionOrder> GetNewSubscriptionOrder(Guid subscriptionOrder);
        Task<ChangeSubscriptionOrder> GetChangeSubscriptionOrder(Guid subscriptionOrder);
        Task<CancelSubscriptionOrder> GetCancelSubscriptionOrder(Guid subscriptionOrder);
    }
}
