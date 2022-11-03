using Common.Enums;
using Common.Interfaces;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository<T> where T : ISubscriptionOrder
    {
        Task<List<ISubscriptionOrder>> GetAllSubscriptionOrdersForCustomer(Guid organizationId);
        Task<int> GetTotalSubscriptionOrdersCountForCustomer(Guid organizationId, IList<SubscriptionOrderTypes>? orderTypes = null, string? phoneNumber = null, bool checkOrderExist = false);
        Task<T> AddSubscriptionOrder(T subscriptionOrder);
        Task<OrderSimSubscriptionOrder> GetOrderSimOrder(Guid subscriptionOrder);
        Task<ActivateSimOrder> GetActivateSimOrder(Guid subscriptionOrder);
        Task<TransferToBusinessSubscriptionOrder> GetTransferToBusinessOrder(Guid subscriptionOrder);
        Task<TransferToPrivateSubscriptionOrder> GetTransferToPrivateOrder(Guid subscriptionOrder);
        Task<NewSubscriptionOrder> GetNewSubscriptionOrder(Guid subscriptionOrder);
        Task<ChangeSubscriptionOrder> GetChangeSubscriptionOrder(Guid subscriptionOrder);
        Task<CancelSubscriptionOrder> GetCancelSubscriptionOrder(Guid subscriptionOrder);
        Task<PagedModel<SubscriptionOrderListItemDTO>> GetAllSubscriptionOrdersForCustomer(Guid organizationId, string? search, IList<SubscriptionOrderTypes>? OrderType, int page, int limit, CancellationToken cancellationToken);
    }
}
