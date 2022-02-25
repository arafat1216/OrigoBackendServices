using AutoMapper;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using System.Text.Json;

namespace SubscriptionManagementServices
{
    public class SubscriptionManagementService : ISubscriptionManagementService
    {
        private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
        private readonly ICustomerSettingsRepository _customerSettingsRepository;
        private readonly IMapper _mapper;
        private readonly TransferSubscriptionDateConfiguration _transferSubscriptionDateConfiguration;

        public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository, ICustomerSettingsRepository customerSettingsRepository, IOptions<TransferSubscriptionDateConfiguration> transferSubscriptionOrderConfigurationOptions, IMapper mapper)
        {
            _subscriptionManagementRepository = subscriptionManagementRepository;
            _customerSettingsRepository = customerSettingsRepository;
            _mapper = mapper;
            _transferSubscriptionDateConfiguration = transferSubscriptionOrderConfigurationOptions.Value;
        }

        public async Task<CustomerOperatorAccountDTO> AddOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, string accountName, int operatorId, Guid callerId)
        {
            var customerOperator = await _subscriptionManagementRepository.GetOperatorAsync(operatorId);

            if (customerOperator == null)
                throw new ArgumentException($"No operator exists with ID {operatorId}");

            var existingCustomerOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(organizationId, accountNumber);

            if (existingCustomerOperatorAccount != null)
                throw new ArgumentException($"A customer operator account with organization ID ({organizationId}) and account number {accountNumber} already exists.");

            var newCustomerOperatorAccount = new CustomerOperatorAccount(organizationId, accountNumber, accountName, operatorId, callerId);

            var operatorAccount = await _subscriptionManagementRepository.AddOperatorAccountForCustomerAsync(newCustomerOperatorAccount);

            return _mapper.Map<CustomerOperatorAccountDTO>(operatorAccount);
        }

        public Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName)
        {
            return Task.FromResult(true);
        }

        public async Task<IEnumerable<CustomerOperatorAccountDTO>> GetAllOperatorAccountsForCustomerAsync(Guid customerId)
        {
            var customerAccount = await _subscriptionManagementRepository.GetAllCustomerOperatorAccountsAsync(customerId);

            List<CustomerOperatorAccountDTO> customerOperatorAccounttDTOs = new();
            customerOperatorAccounttDTOs.AddRange(
                   _mapper.Map<List<CustomerOperatorAccountDTO>>(customerAccount));

            return customerOperatorAccounttDTOs;
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

        public async Task<CustomerSubscriptionProductDTO> AddOperatorSubscriptionProductForCustomerAsync(Guid customerId, int operatorId, string productName, IList<string>? dataPackages, Guid callerId)
        {

            var @operator = await _subscriptionManagementRepository.GetOperatorAsync(operatorId);
            if (@operator == null)
            {
                throw new ArgumentException($"No operator for id {operatorId}");
            }

            var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(customerId);
            if (customerSettings == null)
            {
                customerSettings = new CustomerSettings(customerId, callerId);
            }

            var globalSubscriptionProduct = await _subscriptionManagementRepository.GetSubscriptionProductByNameAsync(productName, operatorId);

            var customerSubscriptionProduct = customerSettings.AddSubscriptionProductAsync(@operator, productName, dataPackages, globalSubscriptionProduct, callerId);
            if (customerSettings.Id > 0)
            {
                await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
            }
            else
            {
                await _customerSettingsRepository.AddCustomerSettingsAsync(customerSettings);
            }
            return _mapper.Map<CustomerSubscriptionProductDTO>(customerSubscriptionProduct);
        }

        public async Task<IList<CustomerSubscriptionProductDTO>> GetAllCustomerSubscriptionProductsAsync(Guid customerId)
        {

            var subscriptionProduct = await _subscriptionManagementRepository.GetAllCustomerSubscriptionProductsAsync(customerId);
            if (subscriptionProduct == null)
            {
                throw new ArgumentException($"Customer with ID {customerId} has no subscription products");
            }
            List<CustomerSubscriptionProductDTO> customerSubscriptionProductDTOs = new();
            customerSubscriptionProductDTOs.AddRange(
                   _mapper.Map<List<CustomerSubscriptionProductDTO>>(subscriptionProduct));
            return customerSubscriptionProductDTOs;
        }
        public async Task<IList<GlobalSubscriptionProductDTO>> GetAllOperatorSubscriptionProductAsync()
        {

            var operatorsSubscriptionProduct = await _subscriptionManagementRepository.GetAllOperatorSubscriptionProducts();
            List<GlobalSubscriptionProductDTO> operatorSubscriptionProducts = new();
            operatorSubscriptionProducts.AddRange(
                _mapper.Map<List<GlobalSubscriptionProductDTO>>(operatorsSubscriptionProduct));

            return operatorSubscriptionProducts;
        }

        public async Task<CustomerSubscriptionProductDTO> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(customerId);
            if (customerSettings == null)
            {
                throw new ArgumentException($"Customer has no customer settings for id {customerId}");
            }

            var removedProduct = customerSettings.RemoveSubscriptionProductAsync(subscriptionId);

            if (removedProduct == null)
            {
                return null;
            }

            await _subscriptionManagementRepository.DeleteOperatorSubscriptionProductForCustomerAsync(removedProduct);

            return _mapper.Map<CustomerSubscriptionProductDTO>(removedProduct);
        }

        public async Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId)
        {
            return await _subscriptionManagementRepository.UpdateOperatorSubscriptionProductForCustomerAsync(customerId, subscriptionId);
        }

        public async Task<TransferToBusinessSubscriptionOrderDTO> TransferPrivateToBusinessSubscriptionOrderAsync(Guid organizationId, TransferToBusinessSubscriptionOrderDTO order)
        {
            var customerOperatorAccount = await _subscriptionManagementRepository.GetCustomerOperatorAccountAsync(organizationId, order.OperatorAccountId.GetValueOrDefault()) ?? new CustomerOperatorAccount();

            if (customerOperatorAccount?.Operator?.OperatorName == order.PrivateSubscription?.OperatorName)
            {
                if (string.IsNullOrEmpty(order.SIMCardNumber))
                    throw new ArgumentException("SIM card number is required.");

                if (order.OrderExecutionDate < DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) || order.OrderExecutionDate > DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                    throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");
            }
            else
            {
                if (!string.IsNullOrEmpty(order.SIMCardNumber))
                {
                    if (order.OrderExecutionDate < DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperator) || order.OrderExecutionDate > DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                        throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperator} workdays ahead or more allowed.");
                }
                else
                {
                    if (order.OrderExecutionDate < DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM) || order.OrderExecutionDate > DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                        throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM} workdays ahead or more is allowed.");
                }
            }

            var subscriptionProduct = await _subscriptionManagementRepository.GetCustomerSubscriptionProductAsync(order.SubscriptionProductId);
            if (subscriptionProduct == null)
                throw new ArgumentException($"No subscription product exists with ID {order.SubscriptionProductId}");

            var dataPackage = await _subscriptionManagementRepository.GetDataPackageAsync(order.DataPackage);
            if (dataPackage == null)
                throw new ArgumentException($"No DataPackage exists with name {order.DataPackage}");

            var subscriptionAddOnProducts = order.AddOnProducts.Select(m => new SubscriptionAddOnProduct(m, order.CallerId));

            //Checking order.CustomerReferenceFields
            var existingFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId);
            foreach (var field in order.CustomerReferenceFields)
            {
                if (!existingFields.Any(m => m.Name == field.Name))
                    throw new ArgumentException($"The field name {field.Name} is not valid for this customer.");
            }


            var subscriptionOrder = await _subscriptionManagementRepository
                .TransferSubscriptionOrderAsync(
                    new TransferToBusinessSubscriptionOrder(
                        order.SIMCardNumber,
                        order.SIMCardAction,
                        order.SubscriptionProductId,
                        organizationId,
                        order.OperatorAccountId,
                        dataPackage.Id,
                        order.OrderExecutionDate,
                        order.MobileNumber,
                        JsonSerializer.Serialize(order.CustomerReferenceFields),
                        subscriptionAddOnProducts.ToList(),
                        order.NewOperatorAccount?.NewOperatorAccountOwner,
                        order.NewOperatorAccount?.NewOperatorAccountPayer,
                        _mapper.Map<PrivateSubscription>(order.PrivateSubscription),
                        _mapper.Map<BusinessSubscription>(order.BusinessSubscription)));

            return _mapper.Map<TransferToBusinessSubscriptionOrderDTO>(subscriptionOrder);


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
