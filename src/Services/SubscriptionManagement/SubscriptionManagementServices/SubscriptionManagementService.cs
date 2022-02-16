using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Models;
using System.Collections.ObjectModel;

namespace SubscriptionManagementServices
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
        private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
        private readonly TransferSubscriptionDateConfiguration _transferSubscriptionDateConfiguration;

        public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository, IOptions<TransferSubscriptionDateConfiguration> transferSubscriptionOrderConfigurationOptions)
        {
            _subscriptionManagementRepository = subscriptionManagementRepository;
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

        public async Task<CustomerSubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages, Guid callerId)
        {
            //Check if operator exist
            var operatorObject = await _subscriptionManagementRepository.GetOperatorAsync(operatorName);

            if (operatorObject == null)
            {
                throw new ArgumentException($"No operator for name {operatorName}");
            }

            //Add subscription product
            var datapackageList = new List<DataPackage>();

            foreach (var dataPackage in datapackages)
            {
                datapackageList.Add(new DataPackage(dataPackage, callerId));
            }

            var subscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductByNameAsync(productName, operatorObject.Id);
            
            CustomerSubscriptionProduct customerProduct;
            if (subscriptionProduct != null)
            {

                customerProduct = new CustomerSubscriptionProduct(subscriptionProduct);

            }
            else
            {
                //Send the subscriptionproduct so the linking happens
                customerProduct = new CustomerSubscriptionProduct(productName,
                                                                        operatorObject,
                                                                        callerId, datapackageList);
            }


            var operatorSetting = await _subscriptionManagementRepository.GetCustomerOperatorSettings(customerId,operatorName);
                
                //If the operator settings is null then add operator to customer
                if (operatorSetting == null)
                {
                    var newListSubs = new List<CustomerSubscriptionProduct>() { customerProduct };
                    await CreateOperatorSettingForCustomer(customerId,newListSubs,operatorObject.Id);
                }
                else //or update the old one
                {
                    operatorSetting.AvailableSubscriptionProducts.Add(customerProduct);
                    await _subscriptionManagementRepository.UpdateCustomerOperatorSettingsAsync(operatorSetting);
                }

            return customerProduct;
        }
        public async Task CreateOperatorSettingForCustomer(Guid customerId, List<CustomerSubscriptionProduct> subs, int operatorId)
        {
            
            var customerOperatorSettings = new CustomerOperatorSettings(operatorId, subs, null);

            await _subscriptionManagementRepository.AddCustomerOperatorSettingsAsync(customerOperatorSettings);
            
            //Check if customer has a setting
            var customersettings = await _subscriptionManagementRepository.GetCustomerSettingsAsync(customerId);
            
            //Make new a new one
            if (customersettings == null)
            {
                var newCustomerSetting = new CustomerSettings(customerId, new List<CustomerOperatorSettings> { customerOperatorSettings }, null);
                await _subscriptionManagementRepository.AddCustomerSettingsAsync(newCustomerSetting);
            }
            else //or add to old one
            {
                customersettings.CustomerOperatorSettings.Add(customerOperatorSettings);
                await _subscriptionManagementRepository.UpdateCustomerSettingsAsync(customersettings);
            }
        }

            public async Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName)
        {
            var subscriptionProduct = await _subscriptionManagementRepository.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);
            if (subscriptionProduct == null)
            {
                throw new ArgumentException($"No subscription products for customer with ID {customerId} and operator {operatorName}");
            }
            return subscriptionProduct;

        }
        public async Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForSettingsAsync(Guid customerId, string operatorName)
        {
            var operatorsSubscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductForOperatorAsync(operatorName);
            if (operatorsSubscriptionProduct == null)
            {
                throw new ArgumentException($"No subscription products for operator {operatorName}");
            }

            var availableSubscriptionProductsForCustomer = await _subscriptionManagementRepository.GetOperatorSubscriptionProductForCustomerAsync(customerId, operatorName);

            if (availableSubscriptionProductsForCustomer == null)
            {
                return operatorsSubscriptionProduct;
            }

            var allProducts = operatorsSubscriptionProduct
                .Union(availableSubscriptionProductsForCustomer)
                .ToList();

            if (allProducts == null)
            {
                throw new ArgumentException($"could not concat subscription product lists");
            }
            return allProducts;
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

            var dataPackage = await _subscriptionManagementRepository.GetDatapackageAsync(datapackageId);
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
