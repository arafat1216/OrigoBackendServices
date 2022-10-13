using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class OperatorRepository : IOperatorRepository
    {
        private readonly SubscriptionManagementContext _subscriptionContext;

        public OperatorRepository(SubscriptionManagementContext subscriptionContext)
        {
            _subscriptionContext = subscriptionContext;
        }

        public async Task<Operator?> GetOperatorAsync(int id)
        {
            return await _subscriptionContext.Operators.FindAsync(id);
        }

        public async Task<IList<Operator>> GetAllOperatorsAsync()
        {
            return await _subscriptionContext.Operators.OrderBy(o => o.OperatorName).ToListAsync();
        }
    }
}