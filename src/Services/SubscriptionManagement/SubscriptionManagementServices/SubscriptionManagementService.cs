using Microsoft.Extensions.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
        private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
        private readonly ILogger<SubscriptionManagementService> _logger;

        public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository, ILogger<SubscriptionManagementService> logger)
        {
            _subscriptionManagementRepository = subscriptionManagementRepository;
            _logger = logger;
        }

        public async Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid customerId, Guid organizationId, string accountNumber, string accountName, int operatorId)
        {
            var newCustomerOperatorAccount = new CustomerOperatorAccount(organizationId, customerId, accountNumber, accountName, operatorId);
            
            return await _subscriptionManagementRepository.AddOperatorAccountForCustomerAsync(newCustomerOperatorAccount);
        }

        public Task<bool> AddOperatorForCustomerAsync(Guid organizationId, IList<string> operators)
        {
            return Task.FromResult(true);
        }

        public Task<bool> AddSubscriptionForCustomerAsync(Guid organizationId)
        {
            return Task.FromResult(true);
        }

        public Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName)
        {
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<CustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId)
        {
            return await _subscriptionManagementRepository.GetAllCustomerOperatorAccountsAsync(customerId);
        }

        public Task<IList<string>> GetAllOperators()
        {
            var operators = new List<string> { "Telenor - NO", "Telia - NO", "Telenor - SE", "Telia - SE" };
            return Task.FromResult<IList<string>>(operators);
        }

        public Task<IList<string>> GetAllOperatorsForCustomer(Guid organizationId)
        {
            var operatorsForCustomer = new List<string> { "Telenor - NO", "Telia - NO" };
            return Task.FromResult<IList<string>>(operatorsForCustomer);
        }

        public async Task<Operator> GetOperator(string operatorName)
        {

            var operatorObject = await _subscriptionManagementRepository.GetOperatorAsync(operatorName);
            return operatorObject;
        }

        public async Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages)
        {
            return await _subscriptionManagementRepository.AddSubscriptionProductForCustomerAsync(customerId, operatorName,productName,datapackages);
        }

        public async Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            return await _subscriptionManagementRepository.GetOperatorSubscriptionProductForCustomerAsync(customerId,operatorName);

        }

        public async Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            return await _subscriptionManagementRepository.DeleteOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionId);
        }

        public async Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            return await _subscriptionManagementRepository.UpdateOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionId);
        }
    }
}
