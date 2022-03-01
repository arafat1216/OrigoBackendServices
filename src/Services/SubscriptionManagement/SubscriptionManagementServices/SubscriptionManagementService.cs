using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices;

public class SubscriptionManagementService : ISubscriptionManagementService
{
    private readonly ISubscriptionManagementRepository _subscriptionManagementRepository;
    private readonly ICustomerSettingsRepository _customerSettingsRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMapper _mapper;
    private readonly TransferSubscriptionDateConfiguration _transferSubscriptionDateConfiguration;
    private readonly IEmailService _emailService;

    public SubscriptionManagementService(ISubscriptionManagementRepository subscriptionManagementRepository,
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

    public async Task<TransferToBusinessSubscriptionOrderDTO> TransferPrivateToBusinessSubscriptionOrderAsync(
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
            if (string.IsNullOrEmpty(order.SIMCardNumber))
                throw new ArgumentException("SIM card number is required.");

            if (order.OrderExecutionDate <
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) ||
                order.OrderExecutionDate >
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                throw new ArgumentException(
                    $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");
        }
        else
        {   //Not ordering a new sim card 
            if (!string.IsNullOrEmpty(order.SIMCardNumber))
            {
                if (order.OrderExecutionDate <
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperator) ||
                    order.OrderExecutionDate >
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                    throw new ArgumentException(
                        $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperator} workdays ahead or more allowed.");
            }
            else
            {
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

        var dataPackage =
            customerSubscriptionProduct.DataPackages.FirstOrDefault(dp => dp.DataPackageName == order.DataPackage);
        if (dataPackage == null)
            throw new ArgumentException($"No DataPackage exists with name {order.DataPackage}");

        var subscriptionAddOnProducts =
            order.AddOnProducts.Select(m => new SubscriptionAddOnProduct(m, order.CallerId));

        //Checking order.CustomerReferenceFields
        var existingFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId);
        foreach (var field in order.CustomerReferenceFields)
            if (existingFields.All(m => m.Name != field.Name))
                throw new ArgumentException($"The field name {field.Name} is not valid for this customer.");


        var subscriptionOrder = await _subscriptionManagementRepository.TransferToBusinessSubscriptionOrderAsync(
            new TransferToBusinessSubscriptionOrder(order.SIMCardNumber, order.SIMCardAction,
                order.SubscriptionProductId, organizationId, order.OperatorAccountId, dataPackage.Id,
                order.OrderExecutionDate, order.MobileNumber, JsonSerializer.Serialize(order.CustomerReferenceFields),
                subscriptionAddOnProducts.ToList(), order.NewOperatorAccount?.NewOperatorAccountOwner,
                order.NewOperatorAccount?.NewOperatorAccountPayer,
                _mapper.Map<PrivateSubscription>(order.PrivateSubscription),
                _mapper.Map<BusinessSubscription>(order.BusinessSubscription)));

        await _emailService.SendEmailAsync("Transfer to business order", order);

        return _mapper.Map<TransferToBusinessSubscriptionOrderDTO>(subscriptionOrder);
    }

    private async Task<string> GetNewOperatorName(TransferToBusinessSubscriptionOrderDTO order, CustomerSettings customerSettings)
    {
        string newOperatorName;
        if (order.OperatorAccountId != null)
        {
            var customerOperatorAccount = customerSettings.CustomerOperatorSettings.FirstOrDefault(o =>
                o.CustomerOperatorAccounts.Any(a => a.Id == order.OperatorAccountId));
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
        var added = await _subscriptionManagementRepository.TransferToPrivateSubscriptionOrderAsync(order);

        await _emailService.SendEmailAsync("Transfer to private order", subscriptionOrder);
        
        return _mapper.Map<TransferToPrivateSubscriptionOrderDTO>(added);
    }
}