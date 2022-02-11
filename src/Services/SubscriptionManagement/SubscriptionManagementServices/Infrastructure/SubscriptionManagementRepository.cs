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

        public async Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid customerId)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Where(m => m.CustomerId == customerId).ToListAsync();
        }

        public async Task<Operator?> GetOperatorAsync(string name)
        {
            return await _subscriptionContext.Operators.Where(o => o.OperatorName == name).FirstOrDefaultAsync();

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

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.FindAsync(id);
        }

        public async Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId)
        {
            var customerOperators = await _subscriptionContext.CustomerOperatorAccounts
                .Include(m => m.Operator)
                .Where(m => m.CustomerId == customerId).Select(m => m.Operator).ToListAsync();

            return customerOperators;
        }

        public async Task<IList<SubscriptionProduct>?> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            var subscriptionProuctsForCustomer = await _subscriptionContext.CustomerSettings
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.DataPackages)
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.Operator)
                .Where(c => c.CustomerId == customerId)
                .SelectMany(m => m.CustomerOperatorSettings.Where(m => m.Operator.OperatorName == operatorName))
                .Select(m => m.AvailableSubscriptionProducts)
                        .FirstOrDefaultAsync();

            if (subscriptionProuctsForCustomer == null)
            {
                return null;
            }

            IList<SubscriptionProduct> result = new List<SubscriptionProduct>();
            foreach (var subscriptionProuct in subscriptionProuctsForCustomer)
            {
                
                result.Add(subscriptionProuct);

            }
            return result;
        }

        public async Task<IList<CustomerOperatorSettings>> GetCustomerOperatorSettings(Guid customerId)
        {

            //return await _subscriptionContext.CustomerOperatorSettings
            //    .Where(c => c.CustomerId == customerId)
            //    .ToListAsync();
            throw new NotImplementedException();

        }

        public async Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, SubscriptionProduct subscriptionProduct)
        {
            //_subscriptionContext.SubscriptionProducts.Add(subscriptionProduct);
           
            //_subscriptionContext.CustomerOperatorSettings.Add(subscriptionProduct);
            //return await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            throw new NotImplementedException();
        }

        public Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
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
