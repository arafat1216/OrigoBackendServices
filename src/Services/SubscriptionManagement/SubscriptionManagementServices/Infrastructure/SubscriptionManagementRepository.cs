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
            var existing = await _subscriptionContext.CustomerOperatorAccounts.FirstOrDefaultAsync(m => m.OperatorId == customerOperatorAccount.OperatorId && m.OrganizationId == customerOperatorAccount.OrganizationId);
            if (existing != null)
                return existing;

            var @operator = await _subscriptionContext.Operators.FindAsync(customerOperatorAccount.OperatorId);

            if (@operator == null)
            {
                throw new ArgumentException($"No operator exists with ID {customerOperatorAccount.OperatorId}");
            }

            customerOperatorAccount.Operator = @operator;
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

        public async Task<DataPackage?> GetDataPackageAsync(int id)
        {
            return await _subscriptionContext.DataPackages.FindAsync(id);
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id)
        {
            return await _subscriptionContext.CustomerOperatorAccounts.Include(m => m.Operator).FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid organizationId)
        {
            var customerSettings = await _subscriptionContext
                 .CustomerSettings
                 .Include(m => m.CustomerOperatorSettings)
                 .ThenInclude(m => m.Operator)
                 .FirstOrDefaultAsync(m => m.CustomerId == organizationId);

            if (customerSettings == null || customerSettings.CustomerOperatorSettings == null)
                return new List<Operator> { };

            return customerSettings?.CustomerOperatorSettings?.Select(m => m.Operator).ToList();
        }

        public async Task<IList<CustomerSubscriptionProduct>?> GetAllCustomerSubscriptionProductsAsync(Guid customerId)
        {
            var subscriptionProductsForCustomer = await _subscriptionContext.CustomerSettings
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.DataPackages)
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.Operator)
                .Include(m => m.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                        .ThenInclude(m => m.GlobalSubscriptionProduct)
                .Where(c => c.CustomerId == customerId)
                .AsSplitQuery()
                .SelectMany(m => m.CustomerOperatorSettings)
                .Select(m => m.AvailableSubscriptionProducts).Where(a => a.Count() != 0)
                .ToListAsync();


            if (subscriptionProductsForCustomer == null)
            {
                return null;
            }

            List<CustomerSubscriptionProduct> result = new();
            foreach (var customerSubscriptionProduct in subscriptionProductsForCustomer)
            {
                foreach (var product in customerSubscriptionProduct)
                {
                    result.Add(product);
                }

            }
            return result;
        }
        public async Task<CustomerSubscriptionProduct?> GetAvailableSubscriptionProductForCustomerbySubscriptionIdAsync(Guid customerId, int subscriptionId)
        {

            var subscriptionProductsForCustomer = await GetAllCustomerSubscriptionProductsAsync(customerId);
            if (subscriptionProductsForCustomer == null)
            {
                return null;
            }
            foreach (var product in subscriptionProductsForCustomer)
            {
                if (product.Id == subscriptionId)
                {
                    return product;
                }
            }

            return null;
        }
        public async Task<IList<SubscriptionProduct>?> GetAllOperatorSubscriptionProducts()
        {
            var operatorSubscriptionproducts = await _subscriptionContext.SubscriptionProducts
                .Include(o => o.Operator)
                .Include(d => d.DataPackages)
                .ToListAsync();

            return operatorSubscriptionproducts;
        }

        public async Task<CustomerOperatorSettings> GetCustomerOperatorSettings(Guid customerId, string operatorName)
        {

            var customerOperatorSettings = await _subscriptionContext.CustomerSettings
                .Include(e => e.CustomerOperatorSettings)
                    .ThenInclude(m => m.AvailableSubscriptionProducts)
                .Include(e => e.CustomerOperatorSettings)
                    .ThenInclude(m => m.CustomerOperatorAccounts)
                .Include(e => e.CustomerOperatorSettings)
                    .ThenInclude(m => m.Operator)
                                     .Where(c => c.CustomerId == customerId).Select(e => e.CustomerOperatorSettings.Where(e => e.Operator.OperatorName == operatorName).FirstOrDefault())
                                     .FirstOrDefaultAsync();

            return customerOperatorSettings;
        }

        public async Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(SubscriptionProduct subscriptionProduct)
        {

            var addedSubscriptionProduct = await _subscriptionContext.SubscriptionProducts.AddAsync(subscriptionProduct);
            await _subscriptionContext.SaveChangesAsync();
            return addedSubscriptionProduct.Entity;
        }

        public async Task<CustomerOperatorSettings> AddCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings)
        {

            var addedCustomerOperatorSetting = await _subscriptionContext.CustomerOperatorSettings.AddAsync(customerOperatorSettings);
            await _subscriptionContext.SaveChangesAsync();
            return addedCustomerOperatorSetting.Entity;
        }

        public async Task DeleteOperatorSubscriptionProductForCustomerAsync(CustomerSubscriptionProduct customerSubscriptionProduct)
        {
            _subscriptionContext.Entry(customerSubscriptionProduct).State = EntityState.Deleted;
            await _subscriptionContext.SaveChangesAsync();
        }

        public Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            throw new NotImplementedException();
        }
        public async Task<CustomerOperatorSettings> UpdateCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings)
        {
            var addedCustomerOperatorSetting = _subscriptionContext.CustomerOperatorSettings.Update(customerOperatorSettings);
            await _subscriptionContext.SaveChangesAsync();

            return addedCustomerOperatorSetting.Entity;
        }

        public async Task<PrivateToBusinessSubscriptionOrder> TransferSubscriptionOrderAsync(PrivateToBusinessSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await _subscriptionContext.SaveChangesAsync();
            return added.Entity;
        }

        public async Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(Guid organizationId, string accountNumber)
        {
            return await _subscriptionContext.CustomerOperatorAccounts
                .Include(m => m.SubscriptionOrders)
                .Include(m => m.CustomerOperatorSettings)
                .Include(m => m.TransferSubscriptionOrders)
                .FirstOrDefaultAsync(m => m.OrganizationId == organizationId && m.AccountNumber == accountNumber);
        }

        public async Task DeleteCustomerOperatorAccountAsync(CustomerOperatorAccount customerOperatorAccount)
        {
            if (customerOperatorAccount.SubscriptionOrders.Any() || customerOperatorAccount.TransferSubscriptionOrders.Any())
                throw new ArgumentException("This customer operator accounts cannot be deleted because there are other entities related with it.");

            _subscriptionContext.Remove(customerOperatorAccount);
            await _subscriptionContext.SaveChangesAsync();
        }

        public async Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName, int operatorId)
        {

            var subscriptionProduct = await _subscriptionContext.SubscriptionProducts
                .Include(m => m.DataPackages)
                .FirstOrDefaultAsync(m => m.SubscriptionName == subscriptionProductName && m.OperatorId == operatorId);

            return subscriptionProduct;
        }

        public async Task<IList<Operator>> GetAllOperatorsAsync()
        {
            return await _subscriptionContext.Operators.ToListAsync();
        }

        public async Task<DataPackage?> GetDataPackageAsync(string dataPackageName)
        {
            return await _subscriptionContext.DataPackages.FirstOrDefaultAsync(m => m.DataPackageName == dataPackageName);
        }
    }
}
