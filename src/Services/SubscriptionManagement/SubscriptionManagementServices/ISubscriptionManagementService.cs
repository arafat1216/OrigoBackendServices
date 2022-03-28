using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<TransferToBusinessSubscriptionOrderDTOResponse> TransferPrivateToBusinessSubscriptionOrderAsync(Guid organizationId, TransferToBusinessSubscriptionOrderDTO order);
        Task<TransferToPrivateSubscriptionOrderDTOResponse> TransferToPrivateSubscriptionOrderAsync(Guid organizationId, TransferToPrivateSubscriptionOrderDTO subscriptionOrder);
        Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId);
        Task<ChangeSubscriptionOrderDTO> ChangeSubscriptionOrder(Guid organizationId, NewChangeSubscriptionOrder newChangeSubscriptionOrderDTO);
        Task<CancelSubscriptionOrderDTO> CancelSubscriptionOrder(Guid organizationId, NewCancelSubscriptionOrder subscriptionOrder);
        Task<OrderSimSubscriptionOrderDTO> OrderSim(Guid organizationId, NewOrderSimSubscriptionOrder subscriptionOrder);
        Task<ActivateSimOrderDTO> ActivateSimAsync(Guid organizationId, NewActivateSimOrder simOrder);
        Task<NewSubscriptionOrderDTO> NewSubscriptionOrderAsync(Guid organizationId, NewSubscriptionOrderRequestDTO newSubscriptionOrder);
        Task<DetailViewSubscriptionOrderLog> GetDetailViewSubscriptionOrderLogAsync(Guid organizationId, Guid orderId,int orderType);
    }
}