using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository<T> where T : ISubscriptionOrder
    {
        Task<List<ISubscriptionOrder>> GetAllSubscriptionOrdersForCustomer(Guid organizationId);
        Task<T> AddSubscriptionOrder(T subscriptionOrder);
    }
}
