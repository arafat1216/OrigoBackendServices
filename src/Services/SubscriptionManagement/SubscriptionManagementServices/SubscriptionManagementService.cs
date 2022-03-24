using System.Text.Json;
using AutoMapper;
using Common.Enums;
using Common.Utilities;
using Microsoft.Extensions.Options;
using SubscriptionManagementServices.Exceptions;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
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
       
        var simCardAction = SIMCardValidation.GetSimCardAction(order.SIMCardAction);
        //Same private operator as business operator 
        if (newOperatorName == order.PrivateSubscription?.OperatorName)
        {

            if (string.IsNullOrEmpty(order.SIMCardNumber) && simCardAction == SIMAction.New)
                throw new InvalidSimException($"SIM card number is required.",Guid.Parse("6a522a0f-9af4-4a6f-8bfa-9d70a954f3b0"));


            if (order.OrderExecutionDate <
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) ||
                order.OrderExecutionDate >
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                throw new ArgumentException(
                    $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");

            if (simCardAction == SIMAction.New)
            {
                //Sim Number validation
                if (!SIMCardValidation.ValidateSim(order.SIMCardNumber))
                    throw new InvalidSimException(
                            $"SIM card number not valid {order.SIMCardNumber}",Guid.Parse("adc42940-5aef-4d0b-be14-78304a3b606d"));
                //Sim Action validation
                if (!SIMCardValidation.ValidateSimAction(order.SIMCardAction, false))
                    throw new InvalidSimException(
                            $"SIM card action not valid {order.SIMCardAction}",Guid.Parse("662734a2-d010-431d-bbd9-1360fb3e8656"));
            }
        }
        else
        {   //Not ordering a new sim card
            if (simCardAction == SIMAction.Keep || simCardAction == SIMAction.New)
            {
                if (order.OrderExecutionDate <
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperator) ||
                    order.OrderExecutionDate >
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                    throw new ArgumentException(
                        $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperator} workdays ahead or more allowed.");

                if (!string.IsNullOrEmpty(order.SIMCardNumber) && simCardAction == SIMAction.New)
                {
                    //Sim Number validation
                    if (!SIMCardValidation.ValidateSim(order.SIMCardNumber))
                        throw new InvalidSimException(
                                $"SIM card number not valid {order.SIMCardNumber}",Guid.Parse("6574dcc6-cb01-4f0c-9849-a6299e61da99"));

                    //Sim Action validation
                    if (!SIMCardValidation.ValidateSimAction(order.SIMCardAction, true))
                        throw new InvalidSimException(
                                $"SIM card action not valid {order.SIMCardAction}", Guid.Parse("da55d6d1-9323-4165-8c9a-f114820a922d"));
                }
                else
                {
                    throw new InvalidSimException(
                                $"SIM card action is {simCardAction} and Sim card number is empty",Guid.Parse("a2eb272a-3f60-411a-9f23-fe1c08686655"));
                }
                
            }
            else
            {
                if (simCardAction != SIMAction.Order) throw new ArgumentException($"Ordertype is {order.SIMCardAction} but there is no SIM card number");
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

        var transferToBusinessSubscriptionOrder = new TransferToBusinessSubscriptionOrder(order.SIMCardNumber, order.SIMCardAction, customerSubscriptionProduct
            , organizationId, customerOperatorAccount, order?.DataPackage,
            order.OrderExecutionDate, order.MobileNumber, JsonSerializer.Serialize(order.CustomerReferenceFields),
            subscriptionAddOnProducts.ToList(), order.NewOperatorAccount?.NewOperatorAccountOwner,
            order.NewOperatorAccount?.NewOperatorAccountPayer,
            _mapper.Map<PrivateSubscription>(order.PrivateSubscription),
            _mapper.Map<BusinessSubscription>(order.BusinessSubscription), newOperatorName,
            order.CallerId);
        var subscriptionOrder = await _subscriptionManagementRepository.AddSubscriptionOrder(transferToBusinessSubscriptionOrder);

        await _emailService.SendAsync(subscriptionOrder.OrderType, subscriptionOrder.SubscriptionOrderId, order, new Dictionary<string, string> { { "OperatorName", newOperatorName }, { "SubscriptionProductName", transferToBusinessSubscriptionOrder.SubscriptionProductName } });

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

            //Phone number validation
            if (!PhoneNumberUtility.ValidatePhoneNumber(order.MobileNumber, customerOperatorAccount.Operator.Country))
                throw new InvalidPhoneNumberException(order.MobileNumber, customerOperatorAccount.Operator.Country, Guid.Parse("5a9245c4-9ae3-4bda-9a02-fbc741ca3c9d"));

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
            //Phone number validation
            if (!PhoneNumberUtility.ValidatePhoneNumber(order.MobileNumber, operatorForNewOperatorAccount.Country)) 
                throw new InvalidPhoneNumberException(order.MobileNumber, operatorForNewOperatorAccount.Country, Guid.Parse("5a9245c4-9ae3-4bda-9a02-fbc741ca3c9d"));
        }

        return newOperatorName;
    }

    public async Task<TransferToPrivateSubscriptionOrderDTO> TransferToPrivateSubscriptionOrderAsync(
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

        var order = _mapper.Map<TransferToPrivateSubscriptionOrder>(subscriptionOrder);
        order.OrganizationId = organizationId;
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(order);

        await _emailService.SendAsync(added.OrderType, added.SubscriptionOrderId, order, new Dictionary<string, string> { { "OperatorName", subscriptionOrder.OperatorName }});

        return _mapper.Map<TransferToPrivateSubscriptionOrderDTO>(added);
    }

    public async Task<IList<SubscriptionOrderListItemDTO>> GetSubscriptionOrderLog(Guid organizationId)
    {
        IList<ISubscriptionOrder> subscriptionOrders = await _subscriptionManagementRepository.GetAllSubscriptionOrdersForCustomer(organizationId);
        return _mapper.Map<IList<SubscriptionOrderListItemDTO>>(subscriptionOrders);
    }

    public async Task<ChangeSubscriptionOrderDTO> ChangeSubscriptionOrder(Guid organizationId, NewChangeSubscriptionOrder subscriptionOrder)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);
        if (customerSettings != null)
        {
            var customersOperator = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.OperatorName == subscriptionOrder.OperatorName);

            if (customersOperator != null)
            {

                var @operator = await _operatorRepository.GetOperatorAsync(customersOperator.Operator.Id);

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

        await _emailService.SendAsync(changeSubscriptionOrder.OrderType, added.SubscriptionOrderId, subscriptionOrder, new Dictionary<string, string> { { "OperatorName", subscriptionOrder.OperatorName } });

        return _mapper.Map<ChangeSubscriptionOrderDTO>(added);
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
        if (subscriptionOrder.DateOfTermination <
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) ||
                subscriptionOrder.DateOfTermination >
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
        {
            if (subscriptionOrder.DateOfTermination <
                DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator)) throw new ArgumentException(
                $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");
            else throw new ArgumentException(
                 $"Invalid transfer date. Not more then {_transferSubscriptionDateConfiguration.MaxDaysForAll} workdays ahead is allowed.");
        }

        var cancelSubscriptionOrder = new CancelSubscriptionOrder(subscriptionOrder.MobileNumber,
            subscriptionOrder.DateOfTermination, @operator.OperatorName, organizationId, subscriptionOrder.CallerId);
        var added = await _subscriptionManagementRepository.AddSubscriptionOrder(cancelSubscriptionOrder);

        await _emailService.SendAsync(cancelSubscriptionOrder.OrderType, added.SubscriptionOrderId, subscriptionOrder, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName } });
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

        await _emailService.SendAsync(orderSimSubscriptionOrder.OrderType, added.SubscriptionOrderId, subscriptionOrder, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName } });

        return _mapper.Map<OrderSimSubscriptionOrderDTO>(added);
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

        await _emailService.SendAsync(newActivateSimOrder.OrderType, added.SubscriptionOrderId, simOrder, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName } });

        var mapped = _mapper.Map<ActivateSimOrderDTO>(newActivateSimOrder);
        mapped.OperatorId = @operator.Id;

        return mapped;
    }

    public async Task<NewSubscriptionOrderDTO> NewSubscriptionOrderAsync(Guid organizationId, NewSubscriptionOrderRequestDTO newSubscriptionOrder)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId);

        //Using the customer operator account if there is one
        CustomerOperatorAccount? customerOperatorAccount = null;
        CustomerSubscriptionProduct? customerSubscriptionProduct;
        var @operator = await _operatorRepository.GetOperatorAsync(newSubscriptionOrder.OperatorId);

        if (customerSettings != null)
        {
            if (@operator == null) throw new InvalidOperatorIdInputDataException(newSubscriptionOrder.OperatorId, Guid.Parse("58e42fa5-2d54-400d-baa5-a1c516379542"));

            if (!PhoneNumberUtility.ValidatePhoneNumber(newSubscriptionOrder.MobileNumber, @operator.Country))
                throw new InvalidPhoneNumberException(newSubscriptionOrder.MobileNumber, @operator.Country, Guid.Parse("1b2627d5-90c1-4dc9-ad81-3adeeb02478e"));

            customerOperatorAccount = customerSettings.CustomerOperatorSettings.FirstOrDefault(oa => oa.Operator.Id == newSubscriptionOrder.OperatorId)?.CustomerOperatorAccounts.FirstOrDefault(oa => oa.Id == newSubscriptionOrder.OperatorAccountId);
            if (customerOperatorAccount == null && newSubscriptionOrder.NewOperatorAccount == null) throw new CustomerSettingsException($"Customer don't have a customer operator account", Guid.Parse("8ddc95d1-ed32-4daa-9fce-32ad556add6e"));

            customerSubscriptionProduct = customerSettings.CustomerOperatorSettings.FirstOrDefault(o => o.Operator.Id == newSubscriptionOrder.OperatorId)?
                .AvailableSubscriptionProducts.FirstOrDefault(sp => sp.Id == newSubscriptionOrder.SubscriptionProductId);

            if (customerSubscriptionProduct != null)
            {
                if (!string.IsNullOrEmpty(newSubscriptionOrder.DataPackage))
                {
                    var datapackages = customerSubscriptionProduct.DataPackages.FirstOrDefault(dp => dp.DataPackageName == newSubscriptionOrder.DataPackage);
                    if (datapackages == null) throw new CustomerSettingsException($"Customer don't have data package {newSubscriptionOrder.DataPackage} with subscription id {newSubscriptionOrder.SubscriptionProductId}", Guid.Parse("26b43ecc-9315-4099-af46-5d8868ced778"));

                }
            }
            else
            {
                throw new CustomerSettingsException($"Customer don't have subscription with id {newSubscriptionOrder.SubscriptionProductId} for operator with id {newSubscriptionOrder.OperatorId}", Guid.Parse("592e923f-4c62-4c13-8971-0e00f6e72b9e"));
            }
        }
        else
        {
            throw new CustomerSettingsException($"Customer dont have setting", Guid.Parse("3186539a-6897-4e89-9df5-0d5a0d056322"));
        }

        var simCardAction = SIMCardValidation.GetSimCardAction(newSubscriptionOrder.SimCardAction);

        if (simCardAction == 0 || simCardAction == default) throw new InvalidSimException($"Sim card action {newSubscriptionOrder.SimCardAction} not valid", Guid.Parse("aa5e4edd-d0c1-4ce3-b808-a919e4ded5ad"));

        if (simCardAction == SIMAction.Order)
        {
            if (newSubscriptionOrder.SimCardAddress == null) throw new InvalidSimException($"Sim card address needs to be filled in when action is {newSubscriptionOrder.SimCardAction}", Guid.Parse("5ba2e574-51f2-48dc-b9a8-0dea7af598f3"));

            if (newSubscriptionOrder.OrderExecutionDate <
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM) ||
                    newSubscriptionOrder.OrderExecutionDate >
                    DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                throw new ArgumentException(
                    $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForNewOperatorWithSIM} workdays ahead or more is allowed.");
        }


        if (simCardAction == SIMAction.New)
        {
            if (!SIMCardValidation.ValidateSim(newSubscriptionOrder.SimCardNumber)) throw new InvalidSimException($"Sim card number {newSubscriptionOrder.SimCardNumber} is not valid", Guid.Parse("8779d13a-a355-41d8-9f83-dab6cb3cfd53"));

            if (newSubscriptionOrder.OrderExecutionDate <
               DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator) ||
               newSubscriptionOrder.OrderExecutionDate >
               DateTime.UtcNow.AddDays(_transferSubscriptionDateConfiguration.MaxDaysForAll))
                throw new ArgumentException(
                    $"Invalid transfer date. {_transferSubscriptionDateConfiguration.MinDaysForCurrentOperator} workday ahead or more is allowed.");
        }



        var subscriptionAddOnProducts = newSubscriptionOrder.AddOnProducts.Select(ap => new SubscriptionAddOnProduct(ap, newSubscriptionOrder.CallerId));


        var existingFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId);
        foreach (var field in newSubscriptionOrder.CustomerReferenceFields)
            if (existingFields.All(m => m.Name != field.Name))
            {
                throw new CustomerReferenceFieldMissingException(field.Name, new Guid("458d37f4-857f-42d5-b4c2-b617f6c8eb1d"));
            }


        var newSubscription = new NewSubscriptionOrder(organizationId, newSubscriptionOrder.MobileNumber,
                                                       newSubscriptionOrder.OperatorId,
                                                       customerOperatorAccount,
                                                       newSubscriptionOrder.NewOperatorAccount?.NewOperatorAccountOwner,
                                                       newSubscriptionOrder.NewOperatorAccount?.NewOperatorAccountPayer,
                                                       customerSubscriptionProduct.SubscriptionName,
                                                       newSubscriptionOrder.DataPackage,
                                                       newSubscriptionOrder.OrderExecutionDate,
                                                       newSubscriptionOrder.SimCardNumber,
                                                       newSubscriptionOrder.SimCardAction,
                                                       _mapper.Map<SimCardAddress>(newSubscriptionOrder.SimCardAddress),
                                                       JsonSerializer.Serialize(newSubscriptionOrder.CustomerReferenceFields),
                                                       subscriptionAddOnProducts.ToList(),
                                                       _mapper.Map<PrivateSubscription>(newSubscriptionOrder.PrivateSubscription),
                                                       _mapper.Map<BusinessSubscription>(newSubscriptionOrder.BusinessSubscription),
                                                       newSubscriptionOrder.CallerId);

        var subscriptionOrder = await _subscriptionManagementRepository.AddSubscriptionOrder(newSubscription);
        await _emailService.SendAsync(newSubscription.OrderType, subscriptionOrder.SubscriptionOrderId, new Dictionary<string, string> { { "OperatorName", @operator.OperatorName }, { "SubscriptionProductName", newSubscription.SubscriptionProductName } });
        return _mapper.Map<NewSubscriptionOrderDTO>(subscriptionOrder);
    }

    public async Task<DetailViewSubscriptionOrderLog> GetDetailViewSubscriptionOrderLogAsync(Guid organizationId, Guid orderId, int orderType)
    {
        DetailViewSubscriptionOrderLog detailViewSubscriptionOrder;

        switch (orderType)
        {
            //OrderSim
            case 1:
                var orderSim = await _subscriptionManagementRepository.GetOrderSimOrder(orderId);
                var mappedOrderSim =  _mapper.Map<OrderSimSubscriptionOrderDTO>(orderSim);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedOrderSim);
                break;

            //ActivateSimCard
            case 2:
                var activateSim = await _subscriptionManagementRepository.GetActivateSimOrder(orderId);
                var mappedactivateSim = _mapper.Map<ActivateSimOrderDTOResponse>(activateSim);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedactivateSim);
                break;

            //TransferToBusiness
            case 3:
                var transferToBusiness = await _subscriptionManagementRepository.GetTransferToBusinessOrder(orderId);
                var mappedT2B = _mapper.Map<TransferToBusinessSubscriptionOrderDTOResponse>(transferToBusiness);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedT2B);
                break;

            //TransferToPrivate
            case 4:
                var transferToPrivate = await _subscriptionManagementRepository.GetTransferToPrivateOrder(orderId);
                var mappedT2P = _mapper.Map<TransferToPrivateSubscriptionOrderDTOResponse>(transferToPrivate);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedT2P);
                break;

            //New Subscription
            case 5:
                var newSubscription = await _subscriptionManagementRepository.GetNewSubscriptionOrder(orderId);
                var mappedNewSubscription = _mapper.Map<NewSubscriptionOrderDTO>(newSubscription);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedNewSubscription);
                break;

            //ChangeSubscription
            case 6:
                var changeSubscription = await _subscriptionManagementRepository.GetChangeSubscriptionOrder(orderId);
                var mappedChangeSubscription = _mapper.Map<ChangeSubscriptionOrderDTO>(changeSubscription);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedChangeSubscription);
                break;

            //CancelSubscription - operator id does not map
            case 7:
                var cancelSubscription = await _subscriptionManagementRepository.GetCancelSubscriptionOrder(orderId);
                var mappedCancelSubscription = _mapper.Map<CancelSubscriptionOrderDTOResponse>(cancelSubscription);
                detailViewSubscriptionOrder = _mapper.Map<DetailViewSubscriptionOrderLog>(mappedCancelSubscription);
                break;
 
            default:
                throw new ArgumentOutOfRangeException("Could not find ordertype");
                
        }
        return detailViewSubscriptionOrder;
    }
}