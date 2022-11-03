using Common.Enums;
using Common.Interfaces;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<TransferToBusinessSubscriptionOrderDTOResponse> TransferPrivateToBusinessSubscriptionOrderAsync(Guid organizationId, TransferToBusinessSubscriptionOrderDTO order);
        Task<TransferToPrivateSubscriptionOrderDTOResponse> TransferToPrivateSubscriptionOrderAsync(Guid organizationId, TransferToPrivateSubscriptionOrderDTO subscriptionOrder);
        Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId);
        Task<int> GetSubscriptionOrdersCount(Guid organizationId, IList<SubscriptionOrderTypes>? orderTypes = null, string? phoneNumber = null, bool checkOrderExist = false);
        Task<ChangeSubscriptionOrderDTO> ChangeSubscriptionOrder(Guid organizationId, NewChangeSubscriptionOrder newChangeSubscriptionOrderDTO);
        Task<CancelSubscriptionOrderDTO> CancelSubscriptionOrder(Guid organizationId, NewCancelSubscriptionOrder subscriptionOrder);
        Task<OrderSimSubscriptionOrderDTO> OrderSim(Guid organizationId, NewOrderSimSubscriptionOrder subscriptionOrder);
        Task<ActivateSimOrderDTO> ActivateSimAsync(Guid organizationId, NewActivateSimOrder simOrder);
        Task<NewSubscriptionOrderDTO> NewSubscriptionOrderAsync(Guid organizationId, NewSubscriptionOrderRequestDTO newSubscriptionOrder);
        Task<DetailViewSubscriptionOrderLog> GetDetailViewSubscriptionOrderLogAsync(Guid organizationId, Guid orderId,int orderType);
        Task<PagedModel<SubscriptionOrderListItemDTO>> GetAllSubscriptionOrderLog(Guid organizationId, string? search, IList<SubscriptionOrderTypes>? OrderType, int page, int limit, CancellationToken cancellationToken);
    }
}