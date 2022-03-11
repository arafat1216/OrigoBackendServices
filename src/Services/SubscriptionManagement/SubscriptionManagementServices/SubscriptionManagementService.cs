using System.Text.Json;
using AutoMapper;
using Common.Exceptions;
using Common.Utilities;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Exceptions;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Utilities;

namespace SubscriptionManagementServices;

public class SubscriptionManagementService : ISubscriptionManagementService
{
    private readonly ISubscriptionManagementRepository<ISubscriptionOrder>  _subscriptionManagementRepository;
    private readonly ICustomerSettingsRepository _customerSettingsRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMapper _mapper;
    private readonly TransferSubscriptionDateConfiguration _transferSubscriptionDateConfiguration;
    private readonly IEmailService _emailService;

    public SubscriptionManagementService(ISubscriptionManagementRepository<ISubscriptionOrder> subscriptionManagementRepository,
        ICustomerSettingsRepository customerSettingsRepository,
        IOperatorRepository operatorRepository,
        IOptions<TransferSubscriptionDateConfiguration> transferSubscriptionOrderConfigurationOptions, 
        IMapper mapper,
        IEmailService emailService)
    {
        _subscriptionManagementRepository = subscriptionManagementRepository;
        _customerSettingsRepository = customerSettingsRepository;
        _operatorRepository = operatorRepository;
        _mapper = mapper;
        _transferSubscriptionDateConfiguration = transferSubscriptionOrderConfigurationOptions.Value;
        _emailService = emailService;
    }

    public async Task<TransferToBusinessSubscriptionOrderDTOResponse> TransferPrivateToBusinessSubscriptionOrderAsync(
        Guid organizationId, TransferToBusinessSubscriptionOrderDTO order)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
        if (customerSettings == null) throw new ArgumentException($"Customer {organizationId} not configured.");

        if (order.OperatorAccountId == null && order.NewOperatorAccount == null)
            throw new ArgumentException("Operator account id or new operator information must be provided.");

        var newOperatorName = await GetNewOperatorName(order, customerSettings);
        //Same private operator as business operator 
        if (newOperatorName == order.PrivateSubscription?.OperatorName)
        {
            
            if (string.IsNullOrEmpty(order.SIMCardNumber) && order.SIMCardAction != "Order")
                throw new ArgumentException("SIM card number is required.");
            

            if (order.OrderExecutionDate <
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) ||
                order.OrderExecutionDate >
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                throw new ArgumentException(
                    $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");

            if (order.SIMCardAction != "Order") { 
            //Sim Number validation
            if (!SIMCardValidation.ValidateSim(order.SIMCardNumber))
                throw new ArgumentException(
                        $"SIM card number not valid {order.SIMCardNumber}");
            //Sim Action validation
            if (!SIMCardValidation.ValidateSimAction(order.SIMCardAction, false))
                throw new ArgumentException(
                        $"SIM card action not valid {order.SIMCardAction}");
            }
        }
        else
        {   //Not ordering a new sim card
            if (!string.IsNullOrEmpty(order.SIMCardNumber) && order.SIMCardAction != "Order")
            {
                if (order.OrderExecutionDate <
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperator) ||
                    order.OrderExecutionDate >
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                    throw new ArgumentException(
                        $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperator} workdays ahead or more allowed.");

                //Sim Number validation
                if (!SIMCardValidation.ValidateSim(order.SIMCardNumber))
                    throw new ArgumentException(
                            $"SIM card number not valid {order.SIMCardNumber}");

                //Sim Action validation
                if (!SIMCardValidation.ValidateSimAction(order.SIMCardAction, true))
                    throw new ArgumentException(
                            $"SIM card action not valid {order.SIMCardAction}");
            }
            else
            {
                if (order.SIMCardAction != "Order") throw new ArgumentException($"Ordertype is {order.SIMCardAction} but there is no SIM card number");
                //Ordering a new sim card - no need for sim card number
                if (order.OrderExecutionDate <
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM) ||
                    order.OrderExecutionDate >
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                    throw new ArgumentException(
                        $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM} workdays ahead or more is allowed.");
            }
        }

        var customerSubscriptionProduct = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == newOperatorName)
            ?.AvailableSubscriptionProducts.FirstOrDefault(p => p.Id == order.SubscriptionProductId);
        if (customerSubscriptionProduct == null)
            throw new ArgumentException(
                $"No subscription product exists with ID {order.SubscriptionProductId}");

        
        //Datapackages only check if there is a value from request
        if (!string.IsNullOrEmpty(order.DataPackage))
        {
            var dataPackage =
                customerSubscriptionProduct.DataPackages.FirstOrDefault(dp => dp.DataPackageName == order.DataPackage);
            if (dataPackage == null)
                throw new ArgumentException($"No DataPackage exists with name {order.DataPackage}");
        }

        var subscriptionAddOnProducts =
            order.AddOnProducts.Select(m => new SubscriptionAddOnProduct(m, order.CallerId));

        //Checking order.CustomerReferenceFields
        var existingFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId);
        foreach (var field in order.CustomerReferenceFields)
            if (existingFields.All(m => m.Name != field.Name))
                throw new ArgumentException($"The field name {field.Name} is not valid for this customer.");

        CustomerOperatorAccount? customerOperatorAccount = null;

        if (order.OperatorAccountId.HasValue)
        {
            customerOperatorAccount = customerSettings.CustomerOperatorSettings.FirstOrDefault(o =>  o.Operator.OperatorName == newOperatorName)
                ?.CustomerOperatorAccounts.FirstOrDefault(p => p.Id == order.OperatorAccountId);
            if (customerSubscriptionProduct == null)
                throw new ArgumentException(
                    $"No subscription product exists with ID {order.SubscriptionProductId}");
        }

        var transferToBusinessSubscriptionOrder = new TransferToBusinessSubscriptionOrder(order.SIMCardNumber, order.SIMCardAction, customerSubscriptionProduct
            , organizationId, customerOperatorAccount, order?.DataPackage,
            order.OrderExecutionDate, order.MobileNumber, JsonSerializer.Serialize(order.CustomerReferenceFields),
            subscriptionAddOnProducts.ToList(), order.NewOperatorAccount?.NewOperatorAccountOwner,
            order.NewOperatorAccount?.NewOperatorAccountPayer,
            _mapper.Map<PrivateSubscription>(order.PrivateSubscription),
            _mapper.Map<BusinessSubscription>(order.BusinessSubscription),newOperatorName,
            order.CallerId);
        var subscriptionOrder = await _subscriptionManagementRepository.AddSubscriptionOrder(transferToBusinessSubscriptionOrder);

        await _emailService.SendEmailAsync($"[{subscriptionOrder.SubscriptionOrderId}]-[{subscriptionOrder.OrderType}]", subscriptionOrder);

        var mapping = _mapper.Map<TransferToBusinessSubscriptionOrderDTOResponse>(subscriptionOrder); 

        return mapping;
    }

    private async Task<string> GetNewOperatorName(TransferToBusinessSubscriptionOrderDTO order, CustomerSettings customerSettings)
    {
        string newOperatorName;
        if (order.OperatorAccountId != null)
        {
            var customerOperatorAccount = customerSettings.CustomerOperatorSettings.SelectMany(os => os.CustomerOperatorAccounts)
                .FirstOrDefault(oa => oa.Id == order.OperatorAccountId);
            if (customerOperatorAccount == null)
                throw new ArgumentException($"Customer operator account id {order.OperatorAccountId} not found.");
            newOperatorName = customerOperatorAccount.Operator.OperatorName;
        }
        else
        {
            if (order.NewOperatorAccount == null)
                throw new ArgumentException("Operator id must be given for new operator account.");

            var operatorForNewOperatorAccount =
                await _operatorRepository.GetOperatorAsync(order.NewOperatorAccount.OperatorId);
            if (operatorForNewOperatorAccount == null)
                throw new ArgumentException("Operator id for new operator account not found.");
            newOperatorName = operatorForNewOperatorAccount.OperatorName;
        }

        return newOperatorName;
    }

    public async Task<TransferToPrivateSubscriptionOrderDTO> TransferToPrivateSubscriptionOrderAsync(
        Guid organizationId, TransferToPrivateSubscriptionOrderDTO subscriptionOrder)
    {
        var order = _mapper.Map<TransferToPrivateSubscriptionOrder>(subscriptionOrder);
        order.OrganizationId = organizationId;
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(order);

        await _emailService.SendEmailAsync($"[{added.SubscriptionOrderId}]-[{added.OrderType}]", added);

        return _mapper.Map<TransferToPrivateSubscriptionOrderDTO>(added);
    }

    public async Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId)
    {
        IList<ISubscriptionOrder> subscriptionOrders =  await _subscriptionManagementRepository.GetAllSubscriptionOrdersForCustomer(organizationId);
        return _mapper.Map<IList<SubscriptionOrderListItemDTO>>(subscriptionOrders);
    }

    public async Task<ChangeSubscriptionOrderDTO> ChangeSubscriptionOrder(Guid organizationId, NewChangeSubscriptionOrder subscriptionOrder)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
        if (customerSettings != null)
        {
            var customersOperator = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == subscriptionOrder.OperatorName);

            if (customersOperator != null) {

                var @operator = await _operatorRepository.GetOperatorAsync(customersOperator.Operator.Id);
                
                if (!PhoneNumberUtility.ValidatePhoneNumber(subscriptionOrder.MobileNumber, @operator?.Country ?? string.Empty))
                {
                    throw new InvalidPhoneNumberException(
                    $"Phone number {subscriptionOrder.MobileNumber} not valid for countrycode {@operator?.Country}.");
                }

                var subscriptionProduct = customersOperator.AvailableSubscriptionProducts.FirstOrDefault(sp => sp.SubscriptionName == subscriptionOrder.ProductName);
                if (subscriptionProduct != null)
                {
                    if (!String.IsNullOrEmpty(subscriptionOrder.PackageName))
                    {
                        var datapackage = subscriptionProduct.DataPackages.FirstOrDefault(d => d.DataPackageName == subscriptionOrder.PackageName);
                        
                        if (datapackage == null) throw new CustomerSettingsException($"Customer does not have datapackage {subscriptionOrder.PackageName} with product {subscriptionOrder.ProductName} as a setting");
                    }
                }
                else
                {
                    throw new CustomerSettingsException($"Customer does not have product {subscriptionOrder.ProductName} as a setting");
                }
            }
            else
            {
                throw new CustomerSettingsException($"Customer does not have this operator {subscriptionOrder.OperatorName} as a setting");
            }
        }
        else
        {
            throw new CustomerSettingsException($"Customer has no settings");
        }
        
        var changeSubscriptionOrder = new ChangeSubscriptionOrder(subscriptionOrder.MobileNumber, subscriptionOrder.ProductName, subscriptionOrder.PackageName, subscriptionOrder.OperatorName, subscriptionOrder.SubscriptionOwner,organizationId, subscriptionOrder.CallerId);

        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(changeSubscriptionOrder);

        await _emailService.SendEmailAsync(changeSubscriptionOrder.OrderType, changeSubscriptionOrder);

        return _mapper.Map<ChangeSubscriptionOrderDTO>(added); 
    }

    public async Task<CancelSubscriptionOrderDTO> CancelSubscriptionOrder(Guid organizationId, NewCancelSubscriptionOrder subscriptionOrder)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(subscriptionOrder.OperatorId);
        if (@operator == null)
        {
            throw new InvalidOperatorIdInputDataException(
                $"No operator with OperatorId {subscriptionOrder.OperatorId} found");
        }
        var cancelSubscriptionOrder = new CancelSubscriptionOrder(subscriptionOrder.MobileNumber,
            subscriptionOrder.DateOfTermination, @operator.OperatorName, organizationId, subscriptionOrder.CallerId);
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(cancelSubscriptionOrder);

        await _emailService.SendEmailAsync(cancelSubscriptionOrder.OrderType, cancelSubscriptionOrder);

        return _mapper.Map<CancelSubscriptionOrderDTO>(added);
    }

    public async Task<OrderSimSubscriptionOrderDTO> OrderSim(Guid organizationId, NewOrderSimSubscriptionOrder subscriptionOrder)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(subscriptionOrder.OperatorId);
        if (@operator == null)
        {
            throw new InvalidOperatorIdInputDataException(
                $"No operator with OperatorId {subscriptionOrder.OperatorId} found");
        }
        var orderSimSubscriptionOrder = new OrderSimSubscriptionOrder(subscriptionOrder.SendToName, subscriptionOrder.Address.Street,
            subscriptionOrder.Address.Postcode, subscriptionOrder.Address.City, subscriptionOrder.Address.Country, @operator.OperatorName, subscriptionOrder.Quantity, organizationId, subscriptionOrder.CallerId);
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(orderSimSubscriptionOrder);

        await _emailService.SendEmailAsync(orderSimSubscriptionOrder.OrderType, orderSimSubscriptionOrder);

        return _mapper.Map<OrderSimSubscriptionOrderDTO>(added);
    }

    public async Task<ActivateSimOrderDTO> ActivateSimAsync(Guid organizationId, NewActivateSimOrder simOrder)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(simOrder.OperatorId);
        if (@operator == null)
        {
            throw new InvalidOperatorIdInputDataException(
                $"No operator with OperatorId {simOrder.OperatorId} found.");
        }
        
        if (!SIMCardValidation.ValidateSim(simOrder.SimCardNumber))
            throw new InvalidSimException(
                    $"SIM card number: {simOrder.SimCardNumber} not valid.");

        if (!SIMCardValidation.ValidateSimType(simOrder.SimCardType))
            throw new InvalidSimException(
                    $"SIM card type: {simOrder.SimCardType} not valid.");

        if (!PhoneNumberUtility.ValidatePhoneNumber(simOrder.MobileNumber,@operator.Country))
            throw new InvalidPhoneNumberException(
                    $"Phone number {simOrder.MobileNumber} not valid for countrycode {@operator.Country}.");

        var newActivateSimOrder = new ActivateSimOrder(simOrder.MobileNumber, @operator.OperatorName, simOrder.SimCardNumber, simOrder.SimCardType, organizationId, simOrder.CallerId);

        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(newActivateSimOrder);

        await _emailService.SendEmailAsync(newActivateSimOrder.OrderType, newActivateSimOrder);

        var mapped = _mapper.Map<ActivateSimOrderDTO>(newActivateSimOrder);
        mapped.OperatorId = @operator.Id;

        return mapped;
    }
}