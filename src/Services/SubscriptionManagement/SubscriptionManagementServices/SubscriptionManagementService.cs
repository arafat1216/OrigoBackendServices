using AutoMapper;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
        private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
        private readonly IMapper _mapper;
        private readonly TransferSubscriptionDateConfiguration _transferSubscriptionDateConfiguration;

        public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository, IOptions<TransferSubscriptionDateConfiguration> transferSubscriptionOrderConfigurationOptions, IMapper mapper)
        {
            _subscriptionManagementRepository = subscriptionManagementRepository;
            _mapper = mapper;
            _transferSubscriptionDateConfiguration = transferSubscriptionOrderConfigurationOptions.Value;
        }

        public async Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, string accountName, int operatorId, Guid callerId)
        {
            var customerOperator = await _subscriptionManagementRepository.GetOperatorAsync(operatorId);

            if (customerOperator == null)
                throw new ArgumentException($"No operator exists with ID {operatorId}");

            var existingCustomerOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(organizationId, accountNumber);

            if (existingCustomerOperatorAccount != null)
                throw new ArgumentException($"A customer operator account with organization ID ({organizationId}) and account name {accountName} already exists.");

            var newCustomerOperatorAccount = new CustomerOperatorAccount(organizationId, accountNumber, accountName, operatorId, callerId);
            return await _subscriptionManagementRepository.AddOperatorAccountForCustomerAsync(newCustomerOperatorAccount);
        }

        public async Task<SubscriptionOrder> AddSubscriptionOrderForCustomerAsync(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId, string simCardNumber)
        {
            var customerOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(operatorAccountId);
            if (customerOperatorAccount == null)
                throw new ArgumentException($"No operator account exists with ID {operatorAccountId}");

            var subscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductAsync(subscriptionProductId);
            if (subscriptionProduct == null)
                throw new ArgumentException($"No subscription product exists with ID {subscriptionProductId}");

            var dataPackage = await _subscriptionManagementRepository.GetDataPackageAsync(datapackageId);
            if (dataPackage == null)
                throw new ArgumentException($"No DataPackage exists with ID {datapackageId}");

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

        public async Task<IList<Operator>> GetAllOperatorsAsync()
        {
            return await _subscriptionManagementRepository.GetAllOperatorsAsync();
        }

        public async Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId)
        {
            var operatorsForCustomer = await _subscriptionManagementRepository.GetAllOperatorsForCustomerAsync(customerId);
            return operatorsForCustomer;
        }

        public async Task<Operator?> GetOperatorAsync(int id)
        {
            return await _subscriptionManagementRepository.GetOperatorAsync(id);
        }

        public async Task<CustomerSubscriptionProductDTO> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string>? dataPackages, Guid callerId)
        {
            var @operator = await _subscriptionManagementRepository.GetOperatorAsync(operatorName);
            if (@operator == null)
            {
                throw new ArgumentException($"No operator for name {operatorName}");
            }

            var customerSettings = await _subscriptionManagementRepository.GetCustomerSettingsAsync(customerId);
            if (customerSettings == null)
            {
                customerSettings = new CustomerSettings(customerId);
            }

            var globalSubscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductByNameAsync(productName, @operator.Id);
            var customerSubscriptionProduct = customerSettings.AddSubscriptionProductAsync(@operator, productName, dataPackages, globalSubscriptionProduct, callerId);
            if (customerSettings.Id > 0)
            {
                await _subscriptionManagementRepository.UpdateCustomerSettingsAsync(customerSettings);
            }
            else
            {
                await _subscriptionManagementRepository.AddCustomerSettingsAsync(customerSettings);
            }
            return _mapper.Map<CustomerSubscriptionProductDTO>(customerSubscriptionProduct);
        }

        //public async Task CreateOperatorSettingForCustomer(Guid customerId, List<CustomerSubscriptionProduct> subs, int operatorId)
        //{

        //    var customerOperatorSettings = new CustomerOperatorSettings(operatorId, subs, null);

        //    await _subscriptionManagementRepository.AddCustomerOperatorSettingsAsync(customerOperatorSettings);

        //    //Check if customer has a setting
        //    var customerSettings = await _subscriptionManagementRepository.GetCustomerSettingsAsync(customerId);

        //    //Make new a new one
        //    if (customerSettings == null)
        //    {
        //        var newCustomerSetting = new CustomerSettings(customerId, new List<CustomerOperatorSettings> { customerOperatorSettings }, null);
        //        await _subscriptionManagementRepository.AddCustomerSettingsAsync(newCustomerSetting);
        //    }
        //    else //or add to old one
        //    {
        //        customerSettings.CustomerOperatorSettings.Add(customerOperatorSettings);
        //        await _subscriptionManagementRepository.UpdateCustomerSettingsAsync(customerSettings);
        //    }
        //}

        public async Task<IList<CustomerSubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            var subscriptionProduct = await _subscriptionManagementRepository.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);
            if (subscriptionProduct == null)
            {
                throw new ArgumentException($"No subscription products for customer with ID {customerId} and operator {operatorName}");
            }
            return subscriptionProduct;

        }
        public async Task<IList<CustomerSubscriptionProductDTO>> GetOperatorSubscriptionProductForSettingsAsync(Guid customerId, string operatorName)
        {
            var operatorsSubscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductForOperatorAsync(operatorName);
            List<CustomerSubscriptionProductDTO> customerSubscriptionProductDTOs = new();
            customerSubscriptionProductDTOs.AddRange(
                _mapper.Map<List<CustomerSubscriptionProductDTO>>(operatorsSubscriptionProduct));
            var availableSubscriptionProductsForCustomer = await _subscriptionManagementRepository.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);

            if (availableSubscriptionProductsForCustomer == null)
            {
                customerSubscriptionProductDTOs.AddRange(
                    _mapper.Map<List<CustomerSubscriptionProductDTO>>(availableSubscriptionProductsForCustomer));
            }

            return customerSubscriptionProductDTOs;
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

                if (orderExecutionDate < DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) || orderExecutionDate > DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                    throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");
            }
            else
            {
                if (!string.IsNullOrEmpty(simCardNumber))
                {
                    if (orderExecutionDate < DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperator) || orderExecutionDate > DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                        throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperator} workdays ahead or more allowed.");
                }
                else
                {
                    if (orderExecutionDate < DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM) || orderExecutionDate > DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                        throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM} workdays ahead or more is allowed.");
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

            var dataPackage = await _subscriptionManagementRepository.GetDataPackageAsync(datapackageId);
            if (dataPackage == null)
                throw new ArgumentException($"No DataPackage exists with ID {datapackageId}");

            return await _subscriptionManagementRepository.TransferSubscriptionOrderAsync(new TransferSubscriptionOrder(customerId, subscriptionProductId, currentOperatorAccountId, datapackageId, callerId, simCardNumber, orderExecutionDate, newOperatorAccountId));
        }

        public async Task DeleteCustomerOperatorAccountAsync(Guid organizationId, string accountNumber)
        {
            var existing = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(organizationId, accountNumber);

            if (existing == null)
                throw new ArgumentException($"No customer operator account with organization ID ({organizationId}) and account name {accountNumber} exists.");



            await _subscriptionManagementRepository.DeleteCustomerOperatorAccountAsync(existing);
        }
    }
}
