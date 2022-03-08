using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<TransferToBusinessSubscriptionOrderDTO> TransferPrivateToBusinessSubscriptionOrderAsync(Guid organizationId, TransferToBusinessSubscriptionOrderDTO order);
        Task<TransferToPrivateSubscriptionOrderDTO> TransferToPrivateSubscriptionOrderAsync(Guid organizationId, TransferToPrivateSubscriptionOrderDTO subscriptionOrder);
        Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId);
        Task<ChangeSubscriptionOrderDTO> ChangeSubscriptionOrder(Guid organizationId, NewChangeSubscriptionOrder newChangeSubscriptionOrderDTO);
        Task<CancelSubscriptionOrderDTO> CancelSubscriptionOrder(Guid organizationId, NewCancelSubscriptionOrder subscriptionOrder);
        Task<OrderSimSubscriptionOrderDTO> OrderSim(Guid organizationId, NewOrderSimSubscriptionOrder subscriptionOrder);
        Task<ActivateSimOrderDTO> ActivateSimAsync(Guid organizationId, NewActivateSimOrder simOrder);
    }
}