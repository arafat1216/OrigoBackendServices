using Microsoft.EntityFrameworkCore;
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

        public async Task<Operator> GetOperatorAsync(string name)
        {
            return await _subscriptionContext.Operator.Where(o => o.OperatorName == name).FirstOrDefaultAsync();
            
        }
    }
}
