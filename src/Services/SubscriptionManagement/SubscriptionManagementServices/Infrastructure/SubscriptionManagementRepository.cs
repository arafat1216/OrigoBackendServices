using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository : ISubscriptionManagementRepository
    {
        private readonly SubscriptionManagementContext _subscriptionContext;

        public SubscriptionManagementRepository(SubscriptionManagementContext subscriptionContext)
        {
            _subscriptionContext = subscriptionContext;
        }

        public async Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount)
        {
            var added = await _subscriptionContext.AddAsync(customerOperatorAccount);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<Operator?> GetOperatorAsync(int id)
        {
            return await _subscriptionContext.Operators.FindAsync(id);
        }

        public async Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid organizationId)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Where(m => m.OrganizationId == organizationId).ToListAsync();
        }

        public async Task<Operator> GetOperatorAsync(string name)
        {
            return await _subscriptionContext.Operators.Where(o => o.OperatorName == name).FirstOrDefaultAsync();

        }
        public Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            //Needs implementing - Rolf should supply the model for setting
            //var hello = _subscriptionContext.CustomerOperatorSettings.Where(o => o.OperatorName == operatorName).ToListAsync();
            throw new NotImplementedException();
        }

        public Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            //Needs implementing - Rolf should supply the model for setting
            throw new NotImplementedException();
        }

        public async Task<SubscriptionOrder> AddSubscriptionOrderAsync(SubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<SubscriptionProduct?> GetSubscriptionProductAsync(int id)
        {
            return await _subscriptionContext.SubscriptionProducts.FindAsync(id);
        }

        public async Task<Datapackage?> GetDatapackageAsync(int id)
        {
            return await _subscriptionContext.Datapackages.FindAsync(id);
        }
        public Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages)
        {
            //Needs implementing - Rolf should supply the model for setting
            throw new NotImplementedException();
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.FindAsync(id);
        }

        public async Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid organizationId)
        {
            var customerOperators = await _subscriptionContext.CustomerOperatorAccounts
                .Include(m => m.Operator)
                .Where(m => m.OrganizationId == organizationId).Select(m => m.Operator).ToListAsync();

            return customerOperators;
        }
        public Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            //Needs implementing - Rolf should supply the model for setting
            throw new NotImplementedException();
        }

        public async Task<TransferSubscriptionOrder> TransferSubscriptionOrderAsync(TransferSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }
    }
}
