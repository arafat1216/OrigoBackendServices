using Microsoft.Extensions.Logging;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
        //private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
        //private readonly ILogger<SubscriptionManagementService> _logger;

        //public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository, ILogger<SubscriptionManagementService> logger)
        //{
        //    _subscriptionManagementRepository = subscriptionManagementRepository;
        //    _logger = logger;
        //}

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

        public Task<IList<string>> GetOperator(string operatorName)
        {

            //var test = await _subscriptionManagementRepository.GetOperatorAsync(operatorName);
            var operatorObject = new List<string> { "Telenor - NO" };

            return Task.FromResult<IList<string>>(operatorObject);
        }

        public Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages)
        {
           
            Operator newOperator = new Operator(operatorName, "EN");
            SubscriptionProduct subscriptionProduct = new SubscriptionProduct(productName, newOperator, datapackages?.Select(i => new Datapackage(i)).ToList(), null);

            return Task.FromResult(subscriptionProduct);
        }

    }
}
