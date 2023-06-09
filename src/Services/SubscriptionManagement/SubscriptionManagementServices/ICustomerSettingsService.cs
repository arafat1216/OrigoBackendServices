﻿using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ICustomerSettingsService
    {
        Task AddOperatorsForCustomerAsync(Guid organizationId, NewOperatorList operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);

        Task<CustomerReferenceFieldDTO?> DeleteCustomerReferenceFieldsAsync(Guid organizationId, int customerReferenceFieldId);
        Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid organizationId);
        Task<CustomerReferenceFieldDTO> AddCustomerReferenceFieldAsync(Guid organizationId, string name, string type, Guid callerId);

        //Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid organizationId);
        //Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount);

        Task<IList<OperatorDTO>> GetAllOperatorsForCustomerAsync(Guid organizationId);
        //Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName);

        Task<IEnumerable<CustomerOperatorAccountDTO>> GetAllOperatorAccountsForCustomerAsync(Guid organizationId);
        Task<CustomerSubscriptionProductDTO> AddOperatorSubscriptionProductForCustomerAsync(Guid organizationId, int operatorId, string productName, IList<string>? dataPackages, Guid callerId);
        Task<CustomerOperatorAccountDTO> AddOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, string accountName, int operatorId, Guid callerId, string connectedOrganizationNumber);
        Task DeleteCustomerOperatorAccountAsync(Guid organizationId, string accountNumber, int operatorId);
        Task<IList<CustomerSubscriptionProductDTO>> GetAllCustomerSubscriptionProductsAsync(Guid organizationId, bool includeOperator = false);
        Task<IList<GlobalSubscriptionProductDTO>> GetAllOperatorSubscriptionProductAsync();
        Task<CustomerSubscriptionProductDTO?> DeleteOperatorSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionId);
        Task<CustomerSubscriptionProductDTO> UpdateSubscriptionProductForCustomerAsync(Guid organizationId, CustomerSubscriptionProductDTO subscriptionId);
        Task<CustomerStandardPrivateSubscriptionProductDTO> CreateStandardPrivateSubscriptionProductsAsync(Guid organizationId, NewCustomerStandardPrivateSubscriptionProduct standardProduct);
        
        Task<IList<CustomerStandardPrivateSubscriptionProductDTO>> GetStandardPrivateSubscriptionProductsAsync(Guid organizationId);
        Task<CustomerStandardPrivateSubscriptionProductDTO> DeleteStandardPrivateSubscriptionProductsAsync(Guid organizationId, int operatorId, Guid callerId);
       
        //*****************************************************************///
        Task<IList<CustomerStandardBusinessSubscriptionProductDTO>> GetStandardBusinessSubscriptionProductsAsync(Guid organizationId, Guid callerId);
        Task<CustomerStandardBusinessSubscriptionProductDTO> AddStandardBusinessSubscriptionProductsAsync(Guid organizationId, NewCustomerStandardBusinessSubscriptionProduct businessSubscriptionProduct);
        Task<CustomerStandardBusinessSubscriptionProductDTO> DeleteStandardBusinessSubscriptionProductsAsync(Guid organizationId, int operatorId, Guid callerId);
    }
}
