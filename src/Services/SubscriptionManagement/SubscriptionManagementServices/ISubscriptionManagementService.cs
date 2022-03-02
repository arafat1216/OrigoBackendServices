using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<TransferToBusinessSubscriptionOrderDTO> TransferPrivateToBusinessSubscriptionOrderAsync(Guid organizationId, TransferToBusinessSubscriptionOrderDTO order);
        Task<TransferToPrivateSubscriptionOrderDTO> TransferToPrivateSubscriptionOrderAsync(Guid organizationId, TransferToPrivateSubscriptionOrderDTO subscriptionOrder);
        Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId);
    }
}