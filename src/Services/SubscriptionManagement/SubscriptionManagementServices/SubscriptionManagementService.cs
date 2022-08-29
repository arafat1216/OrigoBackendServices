using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AutoMapper;
using Common.Enums;
using Common.Utilities;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Exceptions;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Types;
using SubscriptionManagementServices.Utilities;

namespace SubscriptionManagementServices;

public class SubscriptionManagementService : ISubscriptionManagementService
{
    private readonly ISubscriptionManagementRepository<ISubscriptionOrder> _subscriptionManagementRepository;
    private readonly ICustomerSettingsRepository _customerSettingsRepository;
    private readonly IOperatorRepository _operatorRepository;
    private readonly IMapper _mapper;
    private readonly TransferSubscriptionDateConfiguration _transferSubscriptionDateConfiguration;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _today;


    public SubscriptionManagementService(ISubscriptionManagementRepository<ISubscriptionOrder> subscriptionManagementRepository,
        ICustomerSettingsRepository customerSettingsRepository,
        IOperatorRepository operatorRepository,
        IOptions<TransferSubscriptionDateConfiguration> transferSubscriptionOrderConfigurationOptions,
        IMapper mapper,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider)
    {
        _subscriptionManagementRepository = subscriptionManagementRepository;
        _customerSettingsRepository = customerSettingsRepository;
        _operatorRepository = operatorRepository;
        _mapper = mapper;
        _transferSubscriptionDateConfiguration = transferSubscriptionOrderConfigurationOptions.Value;
        _emailService = emailService;
        _today = dateTimeProvider;
    }

    public async Task<TransferToBusinessSubscriptionOrderDTOResponse> TransferPrivateToBusinessSubscriptionOrderAsync(
        Guid organizationId, TransferToBusinessSubscriptionOrderDTO order)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
        if (customerSettings == null) throw new ArgumentException($"Customer {organizationId} not configured.");

        //Needs to have a value for one of these
        if (order.OperatorAccountId == null && order.NewOperatorAccount == null && order.OperatorAccountPhoneNumber == null)
            throw new ArgumentException("Operator account id, new operator information or operator phone number must be provided.");

        //New operator for business subscription
        var newOperator = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.Id == order.OperatorId);
        if (newOperator == null) throw new CustomerSettingsException($"Customer don't have settings for operator with id {order.OperatorId}", Guid.Parse("3ca14132-1f7e-45de-8e80-804335e82375"));

        var newOperatorName = newOperator?.Operator.OperatorName;

        //Phone number validation
        if (!PhoneNumberUtility.ValidatePhoneNumber(order.MobileNumber, newOperator.Operator.Country))
            throw new InvalidPhoneNumberException(order.MobileNumber, newOperator.Operator.Country, Guid.Parse("5a9245c4-9ae3-4bda-9a02-fbc741ca3c9d"));


        var simCardAction = SIMCardValidation.GetSimCardAction(order.SIMCardAction);
        //Same private operator as business operator 
        if (newOperatorName == order.PrivateSubscription?.OperatorName)
        {

            if (string.IsNullOrEmpty(order.SIMCardNumber) && simCardAction == SIMAction.New)
                throw new InvalidSimException($"SIM card number is required.", Guid.Parse("6a522a0f-9af4-4a6f-8bfa-9d70a954f3b0"));

            if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(order.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForCurrentOperator))
                throw new ArgumentException(
                   $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");

            if (simCardAction == SIMAction.New)
            {
                //Sim Number validation
                if (!SIMCardValidation.ValidateSim(order.SIMCardNumber))
                    throw new InvalidSimException(
                            $"SIM card number not valid {order.SIMCardNumber}", Guid.Parse("adc42940-5aef-4d0b-be14-78304a3b606d"));
                //Sim Action validation
                if (!SIMCardValidation.ValidateSimAction(order.SIMCardAction, false))
                    throw new InvalidSimException(
                            $"SIM card action not valid {order.SIMCardAction}", Guid.Parse("662734a2-d010-431d-bbd9-1360fb3e8656"));
            }
        }
        else
        {   //Not ordering a new sim card
            if (simCardAction == SIMAction.Keep || simCardAction == SIMAction.New)
            {

                if (simCardAction == SIMAction.Keep)
                {
                    if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(order.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForCurrentOperator))
                        throw new ArgumentException(
                            $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workdays ahead or more allowed.");
                }
                if (simCardAction == SIMAction.New)
                {
                    if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(order.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForNewOperator))
                        throw new ArgumentException(
                            $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperator} workdays ahead or more allowed.");
                }

                if (!string.IsNullOrEmpty(order.SIMCardNumber) && simCardAction == SIMAction.New)
                {
                    //Sim Number validation
                    if (!SIMCardValidation.ValidateSim(order.SIMCardNumber))
                        throw new InvalidSimException(
                                $"SIM card number not valid {order.SIMCardNumber}", Guid.Parse("6574dcc6-cb01-4f0c-9849-a6299e61da99"));

                    //Sim Action validation
                    if (!SIMCardValidation.ValidateSimAction(order.SIMCardAction, true))
                        throw new InvalidSimException(
                                $"SIM card action not valid {order.SIMCardAction}", Guid.Parse("da55d6d1-9323-4165-8c9a-f114820a922d"));
                }
                else
                {
                    throw new InvalidSimException(
                                $"SIM card action is {simCardAction} and Sim card number is empty", Guid.Parse("a2eb272a-3f60-411a-9f23-fe1c08686655"));
                }

            }

            else
            {
                if (simCardAction != SIMAction.Order) throw new ArgumentException($"Ordertype is {order.SIMCardAction} but there is no SIM card number");
                if (order.SimCardAddress == null) throw new InvalidSimException(
                               $"SIM card action is {simCardAction} and Sim card address is empty", Guid.Parse("16dc9894-ddb7-448f-ab88-7e64caecf991"));
                //Ordering a new sim card - no need for sim card number
                if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(order.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM))
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
            {
                throw new CustomerReferenceFieldMissingException(field.Name, new Guid("19021914-a132-11ec-9fb7-00155d9e3768"));
            }

        CustomerOperatorAccount? customerOperatorAccount = null;

        if (order.OperatorAccountId.HasValue)
        {
            customerOperatorAccount = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == newOperatorName)
                ?.CustomerOperatorAccounts.FirstOrDefault(p => p.Id == order.OperatorAccountId);
            if (customerSubscriptionProduct == null)
                throw new ArgumentException(
                    $"No subscription product exists with ID {order.SubscriptionProductId}");
        }

        var transferToBusinessSubscriptionOrder = new TransferToBusinessSubscriptionOrder(order.SIMCardNumber, order.SIMCardAction, order.SimCardAddress, customerSubscriptionProduct
            , organizationId, customerOperatorAccount, order?.OperatorAccountPhoneNumber, order?.DataPackage,
            order.OrderExecutionDate, order.MobileNumber, JsonSerializer.Serialize(order.CustomerReferenceFields),
            subscriptionAddOnProducts.ToList(), order.NewOperatorAccount?.NewOperatorAccountOwner, order.NewOperatorAccount?.OrganizationNumberOwner,
            order.NewOperatorAccount?.NewOperatorAccountPayer, order.NewOperatorAccount?.OrganizationNumberPayer,
            _mapper.Map<PrivateSubscription>(order.PrivateSubscription),
            _mapper.Map<BusinessSubscription>(order.BusinessSubscription), newOperatorName,
            order.CallerId);
        var subscriptionOrder = await _subscriptionManagementRepository.AddSubscriptionOrder(transferToBusinessSubscriptionOrder);

        await _emailService.SendAsync(OrderTypes.TransferToBusiness.ToString(), subscriptionOrder.SubscriptionOrderId, order, new Dictionary<string, string> { { "OperatorName", newOperatorName }, { "SubscriptionProductName", transferToBusinessSubscriptionOrder.SubscriptionProductName } });

        var operatorSettings = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == newOperatorName);

        var mapping = _mapper.Map<TransferToBusinessSubscriptionOrderDTOResponse>(subscriptionOrder);
        if (mapping != null && operatorSettings != null)
        {
            mapping.OperatorId = operatorSettings.Operator.Id;
            if (mapping.NewOperatorAccount != null) mapping.NewOperatorAccount.OperatorId = operatorSettings.Operator.Id;
            if (mapping.BusinessSubscription != null)
            {
                mapping.BusinessSubscription.OperatorId = operatorSettings.Operator.Id;
                mapping.BusinessSubscription.OperatorName = newOperatorName;
            }
            if (mapping.PrivateSubscription != null) mapping.PrivateSubscription.OperatorId = operatorSettings.Operator.Id;
            if (mapping.OperatorAccountId == null && customerOperatorAccount != null) mapping.OperatorAccountId = customerOperatorAccount.Id;
        }
        return mapping;
    }

    public async Task<TransferToPrivateSubscriptionOrderDTOResponse> TransferToPrivateSubscriptionOrderAsync(
        Guid organizationId, TransferToPrivateSubscriptionOrderDTO subscriptionOrder)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
        if (customerSettings != null)
        {
            var privateStandardProduct = customerSettings.CustomerOperatorSettings.
                FirstOrDefault(o => o.Operator.OperatorName == subscriptionOrder.OperatorName &&
                o.StandardPrivateSubscriptionProduct?.SubscriptionName == subscriptionOrder.NewSubscription);

            if (privateStandardProduct != null)
            {
                var @operator = await _operatorRepository.GetOperatorAsync(privateStandardProduct.Operator.Id);

                if (!PhoneNumberUtility.ValidatePhoneNumber(subscriptionOrder.MobileNumber, @operator?.Country ?? string.Empty))
                {
                    throw new InvalidPhoneNumberException(subscriptionOrder.MobileNumber, @operator?.Country ?? string.Empty, Guid.Parse("4945485a-a30b-11ec-b5fc-00155d8454bd"));
                }

            }
            else
            {
                throw new CustomerSettingsException($"Customer does not have private standard subscription product {subscriptionOrder.NewSubscription} for operator {subscriptionOrder.OperatorName}.", Guid.Parse("0e1fe424-a30b-11ec-acf0-00155d8454bd"));

            }
        }
        else
        {
            throw new CustomerSettingsException($"Customer has no settings", Guid.Parse("30bd977e-a30b-11ec-b545-00155d8454bd"));
        }

        if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(subscriptionOrder.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForCurrentOperator))
            throw new ArgumentException(
                $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workdays ahead or more is allowed.");


        var order = _mapper.Map<TransferToPrivateSubscriptionOrder>(subscriptionOrder);
        order.OrganizationId = organizationId;
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(order);

        await _emailService.SendAsync(OrderTypes.TransferToPrivate.ToString(), added.SubscriptionOrderId, order, new Dictionary<string, string> { { "OperatorName", subscriptionOrder.OperatorName } });
        var mapped = _mapper.Map<TransferToPrivateSubscriptionOrderDTOResponse>(added);
        var operatorSettings = customerSettings?.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == subscriptionOrder.OperatorName);
        if (operatorSettings != null && mapped != null)
        {
            mapped.OperatorId = operatorSettings.Operator.Id;
            if (mapped.PrivateSubscription != null) mapped.PrivateSubscription.OperatorId = operatorSettings.Operator.Id;
        }
        return mapped;
    }

    public async Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId)
    {
        IList<ISubscriptionOrder> subscriptionOrders = await _subscriptionManagementRepository.GetAllSubscriptionOrdersForCustomer(organizationId);
        return _mapper.Map<IList<SubscriptionOrderListItemDTO>>(subscriptionOrders);
    }

    public async Task<ChangeSubscriptionOrderDTO> ChangeSubscriptionOrder(Guid organizationId, NewChangeSubscriptionOrder subscriptionOrder)
    {

        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);

        Operator @operator;

        if (customerSettings != null)
        {
            var customersOperator = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == subscriptionOrder.OperatorName);

            if (customersOperator != null)
            {

                @operator = await _operatorRepository.GetOperatorAsync(customersOperator.Operator.Id);

                if (!PhoneNumberUtility.ValidatePhoneNumber(subscriptionOrder.MobileNumber, @operator?.Country ?? string.Empty))
                {
                    throw new InvalidPhoneNumberException(subscriptionOrder.MobileNumber, @operator?.Country ?? string.Empty, Guid.Parse("4945485a-a30b-11ec-b5fc-00155d8454bd"));
                }

                var subscriptionProduct = customersOperator.AvailableSubscriptionProducts.FirstOrDefault(sp => sp.SubscriptionName == subscriptionOrder.ProductName);
                if (subscriptionProduct != null)
                {
                    if (!String.IsNullOrEmpty(subscriptionOrder.PackageName))
                    {
                        var datapackage = subscriptionProduct.DataPackages.FirstOrDefault(d => d.DataPackageName == subscriptionOrder.PackageName);

                        if (datapackage == null)
                            throw new CustomerSettingsException(
                                $"Customer does not have datapackage {subscriptionOrder.PackageName} with product {subscriptionOrder.ProductName} as a setting",
                                Guid.Parse("23281134-a30b-11ec-bed3-00155d8454bd"));
                    }
                }
                else
                {
                    throw new CustomerSettingsException($"Customer does not have product {subscriptionOrder.ProductName} as a setting", Guid.Parse("0e1fe424-a30b-11ec-acf0-00155d8454bd"));
                }
            }
            else
            {
                throw new CustomerSettingsException($"Customer does not have this operator {subscriptionOrder.OperatorName} as a setting", Guid.Parse("19de70aa-a30b-11ec-aad3-00155d8454bd"));
            }
        }
        else
        {
            throw new CustomerSettingsException($"Customer has no settings", Guid.Parse("30bd977e-a30b-11ec-b545-00155d8454bd"));
        }

        var changeSubscriptionOrder = new ChangeSubscriptionOrder(subscriptionOrder.MobileNumber, subscriptionOrder.ProductName, subscriptionOrder.PackageName, subscriptionOrder.OperatorName, subscriptionOrder.SubscriptionOwner, organizationId, subscriptionOrder.CallerId);

        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(changeSubscriptionOrder);

        await _emailService.SendAsync(OrderTypes.ChangeSubscription.ToString(), added.SubscriptionOrderId, subscriptionOrder, new Dictionary<string, string> { { "OperatorName", subscriptionOrder.OperatorName } });
        var mapped = _mapper.Map<ChangeSubscriptionOrderDTO>(added);
        mapped.OperatorId = @operator.Id;

        return mapped;
    }

    public async Task<CancelSubscriptionOrderDTO> CancelSubscriptionOrder(Guid organizationId, NewCancelSubscriptionOrder subscriptionOrder)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(subscriptionOrder.OperatorId);
        if (@operator == null)
        {
            throw new InvalidOperatorIdInputDataException(subscriptionOrder.OperatorId, Guid.Parse("67b12c32-a30b-11ec-9200-00155d8454bd"));
        }

        if (!PhoneNumberUtility.ValidatePhoneNumber(subscriptionOrder.MobileNumber, @operator.Country))
            throw new InvalidPhoneNumberException(subscriptionOrder.MobileNumber, @operator.Country, Guid.Parse("6bffdff0-a30a-11ec-940a-00155d8454bd"));

        //Date validation
        if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(subscriptionOrder.DateOfTermination), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForCurrentOperator))
            throw new ArgumentException(
                $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");

        var cancelSubscriptionOrder = new CancelSubscriptionOrder(subscriptionOrder.MobileNumber,
            subscriptionOrder.DateOfTermination, @operator.OperatorName, organizationId, subscriptionOrder.CallerId);
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(cancelSubscriptionOrder);

        await _emailService.SendAsync(OrderTypes.ChangeSubscription.ToString(), added.SubscriptionOrderId, subscriptionOrder, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName } });
        var cancelMapped = _mapper.Map<CancelSubscriptionOrderDTO>(added);
        cancelMapped.OperatorId = @operator.Id;

        return cancelMapped;
    }

    public async Task<OrderSimSubscriptionOrderDTO> OrderSim(Guid organizationId, NewOrderSimSubscriptionOrder subscriptionOrder)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(subscriptionOrder.OperatorId);
        if (@operator == null)
        {
            throw new InvalidOperatorIdInputDataException(subscriptionOrder.OperatorId, Guid.Parse("759f2498-a30b-11ec-b131-00155d8454bd"));
        }
        var orderSimSubscriptionOrder = new OrderSimSubscriptionOrder(subscriptionOrder.SendToName, subscriptionOrder.Address.Street,
            subscriptionOrder.Address.Postcode, subscriptionOrder.Address.City, subscriptionOrder.Address.Country, @operator.OperatorName, subscriptionOrder.Quantity, organizationId, subscriptionOrder.CallerId);
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(orderSimSubscriptionOrder);

        await _emailService.SendAsync(OrderTypes.OrderSim.ToString(), added.SubscriptionOrderId, subscriptionOrder, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName } });

        var mapped = _mapper.Map<OrderSimSubscriptionOrderDTO>(added);
        mapped.OperatorId = @operator.Id;

        return mapped;
    }

    public async Task<ActivateSimOrderDTO> ActivateSimAsync(Guid organizationId, NewActivateSimOrder simOrder)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(simOrder.OperatorId);
        if (@operator == null)
        {
            throw new InvalidOperatorIdInputDataException(simOrder.OperatorId, Guid.Parse("8bc6fdc8-a30a-11ec-81d3-00155d8454bd"));
        }

        if (!SIMCardValidation.ValidateSim(simOrder.SimCardNumber))
            throw new InvalidSimException($"SIM card number: {simOrder.SimCardNumber} not valid.", Guid.Parse("ccbd55d4-a30a-11ec-be10-00155d8454bd"));

        if (!SIMCardValidation.ValidateSimType(simOrder.SimCardType))
            throw new InvalidSimException($"SIM card type: {simOrder.SimCardType} not valid.", Guid.Parse("d79a3e18-a30a-11ec-ae47-00155d8454bd"));

        if (!PhoneNumberUtility.ValidatePhoneNumber(simOrder.MobileNumber, @operator.Country))
            throw new InvalidPhoneNumberException(simOrder.MobileNumber, @operator.Country, Guid.Parse("6bffdff0-a30a-11ec-940a-00155d8454bd"));

        var newActivateSimOrder = new ActivateSimOrder(simOrder.MobileNumber, @operator.OperatorName, simOrder.SimCardNumber, simOrder.SimCardType, organizationId, simOrder.CallerId);

        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(newActivateSimOrder);

        await _emailService.SendAsync(OrderTypes.ActivateSim.ToString(), added.SubscriptionOrderId, simOrder, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName } });

        var mapped = _mapper.Map<ActivateSimOrderDTO>(newActivateSimOrder);
        mapped.OperatorId = @operator.Id;

        return mapped;
    }

    public async Task<NewSubscriptionOrderDTO> NewSubscriptionOrderAsync(Guid organizationId, NewSubscriptionOrderRequestDTO newSubscriptionOrder)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);

        if (customerSettings is null)
            throw new CustomerSettingsException($"Customer dont have setting", Guid.Parse("3186539a-6897-4e89-9df5-0d5a0d056322"));

        // Using the customer operator account if there is one
        CustomerOperatorAccount? customerOperatorAccount = null;
        CustomerSubscriptionProduct? customerSubscriptionProduct;
        SimCardAddress? simCardAddress = null;

        var @operator = await _operatorRepository.GetOperatorAsync(newSubscriptionOrder.OperatorId);
        if (@operator is null)
            throw new InvalidOperatorIdInputDataException(newSubscriptionOrder.OperatorId, Guid.Parse("58e42fa5-2d54-400d-baa5-a1c516379542"));


        customerOperatorAccount = customerSettings.CustomerOperatorSettings.FirstOrDefault(oa => oa.Operator.Id == newSubscriptionOrder.OperatorId)?
                                                                           .CustomerOperatorAccounts.FirstOrDefault(oa => oa.Id == newSubscriptionOrder.OperatorAccountId);

        if (customerOperatorAccount is null && newSubscriptionOrder.NewOperatorAccount is null && newSubscriptionOrder.OperatorAccountPhoneNumber is null)
            throw new CustomerSettingsException($"Customer don't have a sufficient billing information", Guid.Parse("8ddc95d1-ed32-4daa-9fce-32ad556add6e"));


        customerSubscriptionProduct = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.Id == newSubscriptionOrder.OperatorId)?
                                                                               .AvailableSubscriptionProducts.FirstOrDefault(sp => sp.Id == newSubscriptionOrder.SubscriptionProductId);

        if (customerSubscriptionProduct is null)
            throw new CustomerSettingsException($"Customer don't have subscription with id {newSubscriptionOrder.SubscriptionProductId} for operator with id {newSubscriptionOrder.OperatorId}", Guid.Parse("592e923f-4c62-4c13-8971-0e00f6e72b9e"));


        if (!string.IsNullOrEmpty(newSubscriptionOrder.DataPackage))
        {
            var datapackages = customerSubscriptionProduct.DataPackages.FirstOrDefault(dp => dp.DataPackageName == newSubscriptionOrder.DataPackage);
            if (datapackages is null)
                throw new CustomerSettingsException($"Customer don't have data package {newSubscriptionOrder.DataPackage} with subscription id {newSubscriptionOrder.SubscriptionProductId}", Guid.Parse("26b43ecc-9315-4099-af46-5d8868ced778"));
        }


        var simCardAction = SIMCardValidation.GetSimCardAction(newSubscriptionOrder.SimCardAction);
        if (simCardAction == 0 || simCardAction == default)
        {
            throw new InvalidSimException($"Sim card action {newSubscriptionOrder.SimCardAction} not valid", Guid.Parse("aa5e4edd-d0c1-4ce3-b808-a919e4ded5ad"));
        }
        else if (simCardAction == SIMAction.Order)
        {
            if (newSubscriptionOrder.SimCardAddress is null)
                throw new InvalidSimException($"Sim card address needs to be filled in when action is {newSubscriptionOrder.SimCardAction}", Guid.Parse("5ba2e574-51f2-48dc-b9a8-0dea7af598f3"));

            if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(newSubscriptionOrder.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM))
                throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM} workdays ahead or more is allowed.");

            simCardAddress = _mapper.Map<SimCardAddress>(newSubscriptionOrder.SimCardAddress);
        }
        else if (simCardAction == SIMAction.New)
        {
            if (newSubscriptionOrder.SimCardNumber is null || !SIMCardValidation.ValidateSim(newSubscriptionOrder.SimCardNumber))
                throw new InvalidSimException($"Sim card number {newSubscriptionOrder.SimCardNumber} is not valid", Guid.Parse("8779d13a-a355-41d8-9f83-dab6cb3cfd53"));

            if (!DateValidator.ValidDateForAction(DateOnly.FromDateTime(newSubscriptionOrder.OrderExecutionDate), DateOnly.FromDateTime(_today.GetNow()), _transferSubscriptionDateConfiguration.MinDaysForCurrentOperator))
                throw new ArgumentException($"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workdays ahead or more is allowed.");
        }


        var subscriptionAddOnProducts = newSubscriptionOrder.AddOnProducts.Select(ap => new SubscriptionAddOnProduct(ap, newSubscriptionOrder.CallerId));
        var existingFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId);

        foreach (var field in newSubscriptionOrder.CustomerReferenceFields)
        {
            if (existingFields.All(m => m.Name != field.Name))
            {
                throw new CustomerReferenceFieldMissingException(field.Name, new Guid("458d37f4-857f-42d5-b4c2-b617f6c8eb1d"));
            }
        }

        var newSubscription = new NewSubscriptionOrder(organizationId,
                                                       newSubscriptionOrder.OperatorId,
                                                       customerOperatorAccount,
                                                       newSubscriptionOrder.NewOperatorAccount?.NewOperatorAccountOwner,
                                                       newSubscriptionOrder.NewOperatorAccount?.OrganizationNumberOwner,
                                                       newSubscriptionOrder.NewOperatorAccount?.NewOperatorAccountPayer,
                                                       newSubscriptionOrder.NewOperatorAccount?.OrganizationNumberPayer,
                                                       newSubscriptionOrder.OperatorAccountPhoneNumber,
                                                       customerSubscriptionProduct.SubscriptionName,
                                                       newSubscriptionOrder.DataPackage,
                                                       newSubscriptionOrder.OrderExecutionDate,
                                                       newSubscriptionOrder.SimCardNumber,
                                                       newSubscriptionOrder.SimCardAction,
                                                       simCardAddress,
                                                       JsonSerializer.Serialize(newSubscriptionOrder.CustomerReferenceFields),
                                                       subscriptionAddOnProducts.ToList(),
                                                       _mapper.Map<PrivateSubscription>(newSubscriptionOrder.PrivateSubscription),
                                                       _mapper.Map<BusinessSubscription>(newSubscriptionOrder.BusinessSubscription),
                                                       newSubscriptionOrder.CallerId);

        var subscriptionOrder = await _subscriptionManagementRepository.AddSubscriptionOrder(newSubscription);

        // TODO: This is a dirty "quickfix" for adding in some of the missing values to the email. This needs to be properly redone ASAP!
        Dictionary<string, string> emailVariables = new()
        {
            { "PrivateSubscription.FirstName", newSubscription.PrivateSubscription?.FirstName ?? "N/A" },
            { "PrivateSubscription.LastName", newSubscription.PrivateSubscription?.LastName ?? "N/A" },
            { "PrivateSubscription.BirthDate", newSubscription.PrivateSubscription?.BirthDate.ToString("yyyy-MM-dd") ?? "N/A" },
            { "PrivateSubscription.Address", newSubscription.PrivateSubscription?.Address ?? "N/A" },
            { "PrivateSubscription.PostalCode", newSubscription.PrivateSubscription?.PostalCode ?? "N/A" },
            { "PrivateSubscription.PostalPlace", newSubscription.PrivateSubscription?.PostalPlace ?? "N/A" },
            { "PrivateSubscription.Country", newSubscription.PrivateSubscription?.Country ?? "N/A" },
            { "PrivateSubscription.Email", newSubscription.PrivateSubscription?.Email ?? "N/A" },

            { "BusinessSubscription.Name", newSubscription.BusinessSubscription?.Name ?? string.Empty },
            { "BusinessSubscription.OrganizationNumber", newSubscription.BusinessSubscription?.OrganizationNumber ?? customerOperatorAccount?.ConnectedOrganizationNumber ?? "N/A" },
            { "BusinessSubscription.Address", newSubscription.BusinessSubscription?.Address ?? "N/A" },
            { "BusinessSubscription.PostalCode", newSubscription.BusinessSubscription?.PostalCode ?? "N/A" },
            { "BusinessSubscription.PostalPlace", newSubscription.BusinessSubscription?.PostalPlace ?? "N/A" },
            { "BusinessSubscription.Country", newSubscription.BusinessSubscription?.Country ?? "N/A" },

            { "OperatorName", @operator.OperatorName },
            { "OperatorAccountPayer", newSubscription.OperatorAccountPayer ?? "N/A" },
            { "OperatorAccountName", newSubscription.OperatorAccountName ?? "N/A" },
            { "SimCardAction", newSubscription.SimCardAction },
            { "SimCardNumber", newSubscription.SimCardNumber ?? "N/A" },

            { "SimCardAddress.FirstName", newSubscription.SimCardReceiverFirstName ?? "N/A" },
            { "SimCardAddress.LastName", newSubscription.SimCardReceiverLastName ?? "N/A" },
            { "SimCardAddress.Address", newSubscription.SimCardReceiverAddress ?? "N/A" },
            { "SimCardAddress.PostalCode", newSubscription.SimCardReceiverPostalCode ?? "N/A" },
            { "SimCardAddress.PostalPlace", newSubscription.SimCardReceiverPostalPlace ?? "N/A" },
            { "SimCardAddress.Country", newSubscription.SimCardReceiverCountry ?? "N/A" },

            { "SubscriptionProductName", newSubscription.SubscriptionProductName },
            { "DataPackageName", newSubscription.DataPackageName ?? "N/A" },
            { "SubscriptionAddOnProducts", string.Join(", ", newSubscriptionOrder.AddOnProducts) },
            { "OrderExecutionDate", newSubscription.ExecutionDate.ToString("yyyy-MM-dd") },

            { "CustomerReferenceFields", newSubscription.CustomerReferenceFields }
        };

        await _emailService.SendAsync(OrderTypes.NewSubscription.ToString(), subscriptionOrder.SubscriptionOrderId, emailVariables);
        var mapped = _mapper.Map<NewSubscriptionOrderDTO>(subscriptionOrder);

        if (mapped != null)
        {
            mapped.OperatorName = @operator.OperatorName;
            if (mapped.PrivateSubscription != null) mapped.PrivateSubscription.OperatorId = @operator.Id;
            if (mapped.BusinessSubscription != null)
            {
                mapped.BusinessSubscription.OperatorName = @operator.OperatorName;
                mapped.BusinessSubscription.OperatorId = @operator.Id;
            }
            if (mapped.NewOperatorAccount != null) mapped.NewOperatorAccount.OperatorName = @operator.OperatorName;
            if (mapped.OperatorAccountId == null && customerOperatorAccount != null) mapped.OperatorAccountId = customerOperatorAccount.Id;

        }

        return mapped;
    }

    public async Task<DetailViewSubscriptionOrderLog> GetDetailViewSubscriptionOrderLogAsync(Guid organizationId, Guid orderId, int orderType)
    {
        DetailViewSubscriptionOrderLog detailViewSubscriptionOrder;
        CustomerOperatorSettings @operator;
        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
        if (customerSetting == null)
            throw new CustomerSettingsException($"Customer {organizationId} is not configured.", Guid.Parse("b0cdc324-f97a-4749-87dd-6a0e18a9aad6"));


        switch ((OrderTypes)orderType)
        {
            case OrderTypes.OrderSim:
                var orderSim = await _subscriptionManagementRepository.GetOrderSimOrder(orderId);
                var mappedOrderSim = _mapper.Map<OrderSimSubscriptionOrderDTO>(orderSim);
                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == orderSim.OperatorName);
                if (@operator != null) mappedOrderSim.OperatorId = @operator.Operator.Id;
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedOrderSim);
                detailViewSubscriptionOrder.CreatedBy = orderSim.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = orderSim.CreatedDate;
                break;

            case OrderTypes.ActivateSim:
                var activateSim = await _subscriptionManagementRepository.GetActivateSimOrder(orderId);
                var mappedactivateSim = _mapper.Map<ActivateSimOrderDTO>(activateSim);

                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == activateSim.OperatorName);
                if (@operator != null) mappedactivateSim.OperatorId = @operator.Operator.Id;

                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedactivateSim);
                detailViewSubscriptionOrder.CreatedBy = activateSim.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = activateSim.CreatedDate;
                break;

            case OrderTypes.TransferToBusiness:
                var transferToBusiness = await _subscriptionManagementRepository.GetTransferToBusinessOrder(orderId);
                var mappedT2B = _mapper.Map<TransferToBusinessSubscriptionOrderDTOResponse>(transferToBusiness);
                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == transferToBusiness.OperatorName);
                if (@operator != null)
                {
                    if (mappedT2B.PrivateSubscription != null) mappedT2B.PrivateSubscription.OperatorId = @operator.Operator.Id;
                    if (mappedT2B.BusinessSubscription != null)
                    {
                        mappedT2B.BusinessSubscription.OperatorId = @operator.Operator.Id;
                        mappedT2B.BusinessSubscription.OperatorName = @operator.Operator.OperatorName;
                    }
                    mappedT2B.OperatorId = @operator.Operator.Id;
                    if (mappedT2B.NewOperatorAccount != null) mappedT2B.NewOperatorAccount.OperatorId = @operator.Operator.Id;

                    if (mappedT2B.OperatorAccountNumber != null)
                    {
                        var operatorAccount = customerSetting.CustomerOperatorSettings?.FirstOrDefault()?.CustomerOperatorAccounts.FirstOrDefault(a => a.AccountNumber == mappedT2B.OperatorAccountNumber);
                        mappedT2B.OperatorAccountId = operatorAccount?.Id;
                    }
                }
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedT2B);
                detailViewSubscriptionOrder.CreatedBy = transferToBusiness.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = transferToBusiness.CreatedDate;
                break;

            case OrderTypes.TransferToPrivate:
                var transferToPrivate = await _subscriptionManagementRepository.GetTransferToPrivateOrder(orderId);
                var mappedT2P = _mapper.Map<TransferToPrivateSubscriptionOrderDTOResponse>(transferToPrivate);
                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == transferToPrivate.OperatorName);
                if (@operator != null)
                {
                    mappedT2P.OperatorId = @operator.Operator.Id;
                    if (mappedT2P.PrivateSubscription != null) mappedT2P.PrivateSubscription.OperatorId = @operator.Operator.Id;

                }
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedT2P);
                detailViewSubscriptionOrder.CreatedBy = transferToPrivate.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = transferToPrivate.CreatedDate;
                break;

            case OrderTypes.NewSubscription:
                var newSubscription = await _subscriptionManagementRepository.GetNewSubscriptionOrder(orderId);
                var mappedNewSubscription = _mapper.Map<NewSubscriptionOrderDTO>(newSubscription);

                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.Id == newSubscription.OperatorId);
                if (@operator != null)
                {
                    mappedNewSubscription.OperatorName = @operator.Operator.OperatorName;
                    if (mappedNewSubscription.BusinessSubscription != null)
                    {
                        mappedNewSubscription.BusinessSubscription.OperatorName = @operator.Operator.OperatorName;
                        mappedNewSubscription.BusinessSubscription.OperatorId = @operator?.Operator.Id;
                    }
                    if (mappedNewSubscription.PrivateSubscription != null) mappedNewSubscription.PrivateSubscription.OperatorId = @operator.Operator.Id;
                    if (mappedNewSubscription.NewOperatorAccount != null) mappedNewSubscription.NewOperatorAccount.OperatorName = @operator.Operator.OperatorName;

                    if (mappedNewSubscription.OperatorAccountNumber != null)
                    {
                        var operatorAccount = customerSetting.CustomerOperatorSettings?.FirstOrDefault()?.CustomerOperatorAccounts.FirstOrDefault(a => a.AccountNumber == mappedNewSubscription.OperatorAccountNumber);
                        mappedNewSubscription.OperatorAccountId = operatorAccount?.Id;
                    }
                }

                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedNewSubscription);
                detailViewSubscriptionOrder.CreatedBy = newSubscription.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = newSubscription.CreatedDate;
                break;

            case OrderTypes.ChangeSubscription:
                var changeSubscription = await _subscriptionManagementRepository.GetChangeSubscriptionOrder(orderId);
                var mappedChangeSubscription = _mapper.Map<ChangeSubscriptionOrderDTO>(changeSubscription);
                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == changeSubscription.OperatorName);
                if (@operator != null) mappedChangeSubscription.OperatorId = @operator.Operator.Id;
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedChangeSubscription);
                detailViewSubscriptionOrder.CreatedBy = changeSubscription.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = changeSubscription.CreatedDate;
                break;

            case OrderTypes.CancelSubscription:
                var cancelSubscription = await _subscriptionManagementRepository.GetCancelSubscriptionOrder(orderId);

                var mappedCancelSubscription = _mapper.Map<CancelSubscriptionOrderDTO>(cancelSubscription);
                @operator = customerSetting.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == cancelSubscription.OperatorName);
                if (@operator != null) mappedCancelSubscription.OperatorId = @operator.Operator.Id;

                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedCancelSubscription);
                detailViewSubscriptionOrder.CreatedBy = cancelSubscription.CreatedBy;
                detailViewSubscriptionOrder.CreatedDate = cancelSubscription.CreatedDate;
                break;

            default:
                var sb = new StringBuilder();
                var enumValues = Enum.GetValues((typeof(OrderTypes)));
                foreach (OrderTypes enumValue in (OrderTypes[])enumValues)
                {
                    sb.AppendLine($"{enumValue.ToString()} - {enumValue.GetHashCode().ToString()}");
                }

                throw new ArgumentException($"Could not find ordertype - current ordertypes\n{sb}");

        }
        return detailViewSubscriptionOrder;
    }
}