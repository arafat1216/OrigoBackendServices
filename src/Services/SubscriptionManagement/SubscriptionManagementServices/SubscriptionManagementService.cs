using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
        private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;

        public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository)
        {
            _subscriptionManagementRepository = subscriptionManagementRepository;
        }

        public async Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid customerId, Guid organizationId, string accountNumber, string accountName, int operatorId, Guid callerId)
        {
            var customerOperator = await _subscriptionManagementRepository.GetOperatorAsync(operatorId);

            if (customerOperator == null)
                throw new ArgumentException($"No operator exists with ID {operatorId}");

            var newCustomerOperatorAccount = new CustomerOperatorAccount(organizationId, customerId, accountNumber, accountName, operatorId, callerId);
            return await _subscriptionManagementRepository.AddOperatorAccountForCustomerAsync(newCustomerOperatorAccount);
        }

        public Task<bool> AddOperatorForCustomerAsync(Guid organizationId, IList<string> operators)
        {
            return Task.FromResult(true);
        }

        public async Task<SubscriptionOrder> AddSubscriptionOrderForCustomerAsync(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId, string simCardNumber)
        {
            var customerOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(operatorAccountId);
            if (customerOperatorAccount == null)
                throw new ArgumentException($"No operator account exists with ID {operatorAccountId}");

            var subscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductAsync(subscriptionProductId);
            if (subscriptionProduct == null)
                throw new ArgumentException($"No subscription product exists with ID {subscriptionProductId}");

            var dataPackage = await _subscriptionManagementRepository.GetDatapackageAsync(datapackageId);
            if (dataPackage == null)
                throw new ArgumentException($"No Datapackage exists with ID {datapackageId}");

            return await _subscriptionManagementRepository.AddSubscriptionOrderAsync(new SubscriptionOrder(customerId, subscriptionProductId, operatorAccountId, datapackageId, callerId, simCardNumber));
        }

        public Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName)
        {
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<CustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId)
        {
            return await _subscriptionManagementRepository.GetAllCustomerOperatorAccountsAsync(customerId);
        }

        public Task<IList<string>> GetAllOperatorsAsync()
        {
            var operators = new List<string> { "Telenor - NO", "Telia - NO", "Telenor - SE", "Telia - SE" };
            return Task.FromResult<IList<string>>(operators);
        }

        public async Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId)
        {
            var operatorsForCustomer = await _subscriptionManagementRepository.GetAllOperatorsForCustomerAsync(customerId);
            return operatorsForCustomer;
        }

        public async Task<Operator> GetOperator(string operatorName)
        {

            var operatorObject = await _subscriptionManagementRepository.GetOperatorAsync(operatorName);
            return operatorObject;
        }

        public async Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages, Guid callerId)
        {
            return await _subscriptionManagementRepository.AddSubscriptionProductForCustomerAsync(customerId, operatorName, productName, datapackages);
        }

        public async Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            return await _subscriptionManagementRepository.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);

        }

        public async Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            return await _subscriptionManagementRepository.DeleteOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionId);
        }

        public async Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            return await _subscriptionManagementRepository.UpdateOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionId);
        }

        public async Task<TransferSubscriptionOrder> TransferSubscriptionOrderAsync(Guid customerId, int subscriptionProductId, int currentOperatorAccountId, int datapackageId, Guid callerId, string simCardNumber, DateTime orderExecutionDate, int newOperatorAccountId)
        {
            if (currentOperatorAccountId == newOperatorAccountId)
            {
                if (string.IsNullOrEmpty(simCardNumber))
                    throw new ArgumentException("SIM card number is required.");

                if (orderExecutionDate < DateTime.UtcNow.AddDays(1) || orderExecutionDate > DateTime.UtcNow.AddDays(30))
                    throw new ArgumentException("Invalid transfer date. 1 workday ahead or more is allowed.");
            }
            else
            {
                if (!string.IsNullOrEmpty(simCardNumber))
                {
                    if (orderExecutionDate < DateTime.UtcNow.AddDays(4) || orderExecutionDate > DateTime.UtcNow.AddDays(30))
                        throw new ArgumentException("Invalid transfer date. 4 workdays ahead or more allowed.");
                }
                else
                {
                    if (orderExecutionDate < DateTime.UtcNow.AddDays(10) || orderExecutionDate > DateTime.UtcNow.AddDays(30))
                        throw new ArgumentException("Invalid transfer date. 10 workdays ahead or more is allowed.");
                }
            }

            var currentOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(currentOperatorAccountId);

            if (currentOperatorAccount == null)
            {
                throw new ArgumentException("Invalid current operator account.");
            }

            var newOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(newOperatorAccountId);
            if (newOperatorAccount == null)
            {
                throw new ArgumentException("Invalid new operator account.");
            }

            var subscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductAsync(subscriptionProductId);
            if (subscriptionProduct == null)
                throw new ArgumentException($"No subscription product exists with ID {subscriptionProductId}");

            var dataPackage = await _subscriptionManagementRepository.GetDatapackageAsync(datapackageId);
            if (dataPackage == null)
                throw new ArgumentException($"No Datapackage exists with ID {datapackageId}");

            return await _subscriptionManagementRepository.TransferSubscriptionOrderAsync(new TransferSubscriptionOrder(customerId, subscriptionProductId, currentOperatorAccountId, datapackageId, callerId, simCardNumber, orderExecutionDate, newOperatorAccountId));
        }
    }
}
