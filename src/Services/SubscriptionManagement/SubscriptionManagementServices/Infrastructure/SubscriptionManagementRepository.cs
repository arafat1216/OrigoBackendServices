using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository : ISubscriptionManagementRepository
    {
        private readonly SubscriptionManagmentContext _subscriptionContext;

        public SubscriptionManagementRepository(SubscriptionManagmentContext subscriptionContext)
        {
            _subscriptionContext = subscriptionContext;
        }
    }
}
