﻿using AutoMapper;
using SubscriptionManagementServices.Exceptions;
using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;
using SubscriptionManagementServices.Types;

namespace SubscriptionManagementServices;

public class CustomerSettingsService : ICustomerSettingsService
{
    private readonly IMapper _mapper;
    private readonly ICustomerSettingsRepository _customerSettingsRepository;
    private readonly IOperatorRepository _operatorRepository;

    public CustomerSettingsService(ICustomerSettingsRepository customerSettingsRepository,
        IOperatorRepository operatorRepository, IMapper mapper)
    {
        _mapper = mapper;
        _customerSettingsRepository = customerSettingsRepository;
        _operatorRepository = operatorRepository;
    }

    public async Task AddOperatorsForCustomerAsync(Guid organizationId, NewOperatorList operators)
    {
        await _customerSettingsRepository.AddCustomerOperatorSettingsAsync(organizationId, operators.Operators, operators.CallerId);
    }

    public async Task<IList<OperatorDTO>> GetAllOperatorsForCustomerAsync(Guid organizationId)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, asNoTracking: true);
        return _mapper.Map<List<OperatorDTO>>(customerSettings?.CustomerOperatorSettings.Select(o => o.Operator).OrderBy(co => co.OperatorName));
    }

    public async Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid organizationId)
    {
        var customerReferenceFields = await _customerSettingsRepository.GetCustomerReferenceFieldsAsync(organizationId, true);
        return _mapper.Map<List<CustomerReferenceFieldDTO>>(customerReferenceFields.OrderBy(r => r.Name));
    }

    public async Task<CustomerReferenceFieldDTO> AddCustomerReferenceFieldAsync(Guid organizationId, string name,
        string type, Guid callerId)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeCustomerReferenceFields: true, asNoTracking: false);
        if (customerSettings == null) customerSettings = new CustomerSettings(organizationId, callerId);

        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidCustomerReferenceInputDataException("Value for name not set.", Guid.Parse("a94abe10-a30b-11ec-abc5-00155d8454bd"));
        if (!Enum.TryParse(type, true, out CustomerReferenceTypes customerReferenceFieldType))
            throw new InvalidCustomerReferenceInputDataException("Invalid value for type. Should be 'account' or 'user'.", Guid.Parse("b5e2a5de-a30b-11ec-9d2f-00155d8454bd"));

        var customerReferenceField = new CustomerReferenceField(name, customerReferenceFieldType, callerId);
        customerSettings.AddCustomerReferenceField(customerReferenceField, callerId);
        if (customerSettings.Id > 0)
            await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
        else
            await _customerSettingsRepository.AddCustomerSettingsAsync(customerSettings);
        return _mapper.Map<CustomerReferenceFieldDTO>(customerReferenceField);
    }

    public async Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId)
    {
        await _customerSettingsRepository.DeleteOperatorForCustomerAsync(organizationId, operatorId);
    }

    public async Task<CustomerReferenceFieldDTO?> DeleteCustomerReferenceFieldsAsync(Guid organizationId,
        int customerReferenceFieldId)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeCustomerReferenceFields: true, asNoTracking: false);
        if (customerSettings == null) return null;

        var customerReferenceField = customerSettings.RemoveCustomerReferenceField(customerReferenceFieldId);
        if (customerReferenceField == null) return null;
        await _customerSettingsRepository.DeleteCustomerReferenceFieldForCustomerAsync(customerReferenceField);
        return _mapper.Map<CustomerReferenceFieldDTO>(customerReferenceField);
    }

    public async Task<CustomerOperatorAccountDTO> AddOperatorAccountForCustomerAsync(Guid organizationId,
        string accountNumber, string accountName, int operatorId, Guid callerId, string? connectedOrganizationNumber)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(operatorId);
        if (@operator == null)
            throw new ArgumentException($"No operator exists with ID {operatorId}");

        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator:true, includeCustomerOperatorAccounts:true, asNoTracking: false);

        if (customerSettings == null) customerSettings = new CustomerSettings(organizationId, callerId);

        var operatorAccount =
            customerSettings.AddCustomerOperatorAccount(accountNumber, accountName, @operator, callerId, connectedOrganizationNumber);
        
        if (customerSettings.Id > 0)
            await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
        else
            await _customerSettingsRepository.AddCustomerSettingsAsync(customerSettings);

        return _mapper.Map<CustomerOperatorAccountDTO>(operatorAccount);
    }

    public async Task DeleteCustomerOperatorAccountAsync(Guid organizationId, string accountNumber, int operatorId)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(operatorId);
        if (@operator == null)
            throw new ArgumentException($"No operator exists with ID {operatorId}");

        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator:true, includeCustomerOperatorAccounts: true, asNoTracking: false);
        if (customerSettings == null) return;
        customerSettings.DeleteCustomerOperatorAccountAsync(accountNumber, @operator);
        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
    }

    public async Task<IEnumerable<CustomerOperatorAccountDTO>> GetAllOperatorAccountsForCustomerAsync(
        Guid organizationId)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeCustomerOperatorAccounts: true, asNoTracking: true);
        return _mapper.Map<List<CustomerOperatorAccountDTO>>(customerSettings?.CustomerOperatorSettings.SelectMany(o => o.CustomerOperatorAccounts).OrderBy(o => o.AccountName));
    }

    public async Task<CustomerSubscriptionProductDTO> AddOperatorSubscriptionProductForCustomerAsync(
        Guid organizationId, int operatorId, string productName, IList<string>? dataPackages, Guid callerId)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(operatorId);
        if (@operator == null)
            throw new ArgumentException($"No operator exists with ID {operatorId}");

        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true,
            includeAvailableSubscriptionProducts: true, includeGlobalSubscriptionProduct: true, includeDataPackages: true, asNoTracking: false);

        if (customerSettings == null) customerSettings = new CustomerSettings(organizationId, callerId);

        var globalSubscriptionProduct =
            await _customerSettingsRepository.GetSubscriptionProductByNameAsync(productName, operatorId);

        var customerSubscriptionProduct = customerSettings.AddSubscriptionProduct(@operator, productName, dataPackages,
            globalSubscriptionProduct, callerId);
        if (customerSettings.Id > 0)
            await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
        else
            await _customerSettingsRepository.AddCustomerSettingsAsync(customerSettings);
        return _mapper.Map<CustomerSubscriptionProductDTO>(customerSubscriptionProduct);
    }

    public async Task<CustomerSubscriptionProductDTO?> DeleteOperatorSubscriptionProductForCustomerAsync(
        Guid organizationId, int subscriptionId)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeAvailableSubscriptionProducts: true, includeOperator: true, asNoTracking: false);
        if (customerSettings == null)
            throw new ArgumentException($"Customer has no customer settings for id {organizationId}");

        var removedProduct = customerSettings.RemoveSubscriptionProduct(subscriptionId);

        if (removedProduct == null) return null;

        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);

        return _mapper.Map<CustomerSubscriptionProductDTO>(removedProduct);
    }

    public async Task<CustomerSubscriptionProductDTO> UpdateSubscriptionProductForCustomerAsync(Guid organizationId,
        CustomerSubscriptionProductDTO subscriptionProduct)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeGlobalSubscriptionProduct: true, includeDataPackages: true, asNoTracking: false);
        if (customerSettings == null)
            throw new ArgumentException($"Customer has no customer settings for id {organizationId}");

        var updateSubscriptionProduct = customerSettings.UpdateSubscriptionProduct(subscriptionProduct);
        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSettings);
        return _mapper.Map<CustomerSubscriptionProductDTO>(updateSubscriptionProduct);
    }

    public async Task<IList<CustomerSubscriptionProductDTO>> GetAllCustomerSubscriptionProductsAsync(
        Guid organizationId, bool includeOperator = false)
    {
        var customerSettings = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: includeOperator, includeAvailableSubscriptionProducts: true, includeDataPackages: true, asNoTracking: true, includeGlobalSubscriptionProduct: true);
        if (customerSettings == null) return new List<CustomerSubscriptionProductDTO>();
        var customerSubscriptionProducts =
            customerSettings.CustomerOperatorSettings.SelectMany(o => o.AvailableSubscriptionProducts);
        var subscriptionProducts = customerSubscriptionProducts.ToList();
        return !subscriptionProducts.Any()
            ? new List<CustomerSubscriptionProductDTO>()
            : _mapper.Map<List<CustomerSubscriptionProductDTO>>(subscriptionProducts.OrderBy(p => p.SubscriptionName));
    }

    public async Task<IList<GlobalSubscriptionProductDTO>> GetAllOperatorSubscriptionProductAsync()
    {
        var operatorsSubscriptionProduct = await _customerSettingsRepository.GetAllOperatorSubscriptionProducts(true);
        List<GlobalSubscriptionProductDTO> operatorSubscriptionProducts = new();
        operatorSubscriptionProducts.AddRange(
            _mapper.Map<List<GlobalSubscriptionProductDTO>>(operatorsSubscriptionProduct?.OrderBy(p => p.SubscriptionName)));

        return operatorSubscriptionProducts;
    }

    public async Task<CustomerStandardPrivateSubscriptionProductDTO> CreateStandardPrivateSubscriptionProductsAsync(Guid organizationId, NewCustomerStandardPrivateSubscriptionProduct standardProduct)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(standardProduct.OperatorId);
        if (@operator == null) throw new InvalidOperatorIdInputDataException(standardProduct.OperatorId, Guid.Parse("d1f782a4-248d-4d14-9a17-af96106e5c91"));

        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, includeStandardPrivateSubscriptionProduct: true, asNoTracking: false);

        if (customerSetting == null) throw new CustomerSettingsException($"Missing customer settings for customer with id: {organizationId}",Guid.Parse("2d376d22-980b-43af-8f5b-543c0a66518c"));

        var customerStandardProduct = customerSetting.AddCustomerStandardPrivateSubscriptionProduct(standardProduct);
        
        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSetting);

        var standardPrivateSubscriptionProductDTO =  _mapper.Map<CustomerStandardPrivateSubscriptionProductDTO>(customerStandardProduct);
        standardPrivateSubscriptionProductDTO.OperatorId = standardProduct.OperatorId;
        standardPrivateSubscriptionProductDTO.OperatorName = @operator.OperatorName;

        return standardPrivateSubscriptionProductDTO;
    }

    public async Task<IList<CustomerStandardPrivateSubscriptionProductDTO>> GetStandardPrivateSubscriptionProductsAsync(Guid organizationId)
    {
        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, includeDataPackages: true, includeStandardPrivateSubscriptionProduct: true, asNoTracking: true);
        
        if (customerSetting == null) return new List<CustomerStandardPrivateSubscriptionProductDTO>();
            
        var standardPrivateProduct = customerSetting.CustomerOperatorSettings.Where(a => a.StandardPrivateSubscriptionProduct != null)
                  .Select(emp => new CustomerStandardPrivateSubscriptionProductDTO
                  {
                      DataPackage = emp.StandardPrivateSubscriptionProduct.DataPackage,
                      OperatorName = emp.Operator.OperatorName,
                      OperatorId = emp.Operator.Id,
                      SubscriptionName = emp.StandardPrivateSubscriptionProduct.SubscriptionName
                  }).ToList();
     
        if (standardPrivateProduct == null) throw new CustomerSettingsException($"Customer with id: {organizationId} don't have standard private subscription product set up", Guid.Parse("8cf64bb9-68d6-4dc4-8fec-f05d5dda6583"));

        return standardPrivateProduct;
    }

    public async Task<CustomerStandardPrivateSubscriptionProductDTO> DeleteStandardPrivateSubscriptionProductsAsync(Guid organizationId, int operatorId, Guid callerId)
    {

        var @operator = await _operatorRepository.GetOperatorAsync(operatorId);
        if (@operator == null) throw new InvalidOperatorIdInputDataException(operatorId, Guid.Parse("1ed448d7-ad4f-4971-96a4-f2d39c554c94"));


        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, includeStandardPrivateSubscriptionProduct: true, asNoTracking: false);
        if (customerSetting == null) throw new CustomerSettingsException($"Missing customer settings for customer with id: {organizationId}", Guid.Parse("574e9a9b-a9c8-4a5d-836b-da81199cedc2"));


        var deletedStandardSubscription = customerSetting.RemoveCustomerStandardPrivateSubscriptionProduct(operatorId, callerId);
        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSetting);

        var mapped = _mapper.Map<CustomerStandardPrivateSubscriptionProductDTO>(deletedStandardSubscription);
        mapped.OperatorName = @operator.OperatorName;

        return mapped;
    }

    public async Task<IList<CustomerStandardBusinessSubscriptionProductDTO>> GetStandardBusinessSubscriptionProductsAsync(Guid organizationId, Guid callerId)
    {
        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, includeStandardBusinessSubscriptionProduct: true, asNoTracking: false);
        if (customerSetting == null) throw new CustomerSettingsException($"Missing customer settings for customer with id: {organizationId}", Guid.Parse("9fadfd14-4cd9-44a9-991d-b2be30a20142"));

        var standardBusinessSubscriptionProduct = customerSetting.CustomerOperatorSettings.Where(a => a.StandardBusinessSubscriptionProduct != null)
                 .Select(emp => new CustomerStandardBusinessSubscriptionProductDTO
                 {
                     DataPackage = emp.StandardBusinessSubscriptionProduct.DataPackage,
                     OperatorName = emp.Operator.OperatorName,
                     OperatorId = emp.Operator.Id,
                     SubscriptionName = emp.StandardBusinessSubscriptionProduct.SubscriptionName,
                     AddOnProducts = emp.StandardBusinessSubscriptionProduct.AddOnProducts != null ? emp.StandardBusinessSubscriptionProduct.AddOnProducts.Select(e => e.AddOnProductName).ToList() : null
                 }).ToList();

        if (standardBusinessSubscriptionProduct == null) throw new CustomerSettingsException($"Customer with id: {organizationId} don't have standard business subscription product set up", Guid.Parse("fab65c43-1f65-479a-8443-4c65b037d695"));

        return standardBusinessSubscriptionProduct;

    }

    public async Task<CustomerStandardBusinessSubscriptionProductDTO> AddStandardBusinessSubscriptionProductsAsync(Guid organizationId, NewCustomerStandardBusinessSubscriptionProduct businessSubscriptionProduct)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(businessSubscriptionProduct.OperatorId);
        if (@operator == null) throw new InvalidOperatorIdInputDataException(businessSubscriptionProduct.OperatorId, Guid.Parse("49b9ef5e-8837-4905-b8bb-b828e17ae273"));

        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, includeStandardBusinessSubscriptionProduct: true, asNoTracking: false);

        if (customerSetting == null) throw new CustomerSettingsException($"Missing customer settings for customer with id: {organizationId}", Guid.Parse("a637c4cd-39de-4715-b6d3-71b99e162bd4"));

        var customerStandardProduct = customerSetting.AddCustomerStandardBusinessSubscriptionProduct(businessSubscriptionProduct);

        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSetting);

        var customerStandardProductDTO = _mapper.Map<CustomerStandardBusinessSubscriptionProductDTO>(customerStandardProduct);
        customerStandardProductDTO.OperatorId = @operator.Id;
        customerStandardProductDTO.OperatorName = @operator.OperatorName;

        return customerStandardProductDTO;

    }

    public async Task<CustomerStandardBusinessSubscriptionProductDTO> DeleteStandardBusinessSubscriptionProductsAsync(Guid organizationId, int operatorId, Guid callerId)
    {
        var @operator = await _operatorRepository.GetOperatorAsync(operatorId);
        if (@operator == null) throw new InvalidOperatorIdInputDataException(operatorId, Guid.Parse("627f7bd2-6b8b-4362-bcde-b1ff61ecb7de"));

        var customerSetting = await _customerSettingsRepository.GetCustomerSettingsAsync(organizationId, includeOperator: true, includeStandardBusinessSubscriptionProduct: true, asNoTracking: false);
        if (customerSetting == null) throw new CustomerSettingsException($"Missing customer settings for customer with id: {organizationId}", Guid.Parse("ccae51ea-513c-4e18-adff-08f06697c7a2"));


        var deletedStandardBusinessSubscription = customerSetting.RemoveCustomerStandardBusinessSubscriptionProduct(operatorId, callerId);
        await _customerSettingsRepository.UpdateCustomerSettingsAsync(customerSetting);

        var deletedStandardBusinessSubscriptionDTO = _mapper.Map<CustomerStandardBusinessSubscriptionProductDTO>(deletedStandardBusinessSubscription);
        deletedStandardBusinessSubscriptionDTO.OperatorName = @operator.OperatorName;
        deletedStandardBusinessSubscriptionDTO.OperatorId = @operator.Id;

        return deletedStandardBusinessSubscriptionDTO;
    }
}