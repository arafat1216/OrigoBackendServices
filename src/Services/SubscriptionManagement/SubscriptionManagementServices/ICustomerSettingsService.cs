using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ICustomerSettingsService
    {
        Task AddOperatorsForCustomerAsync(Guid organizationId, IList<int> operators, Guid callerId);
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
        Task<CustomerOperatorAccountDTO> AddOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, string accountName, int operatorId, Guid CallerId);
        Task DeleteCustomerOperatorAccountAsync(Guid organizationId, string accountNumber, int operatorId);
        Task<IList<CustomerSubscriptionProductDTO>> GetAllCustomerSubscriptionProductsAsync(Guid organizationId);
        Task<IList<GlobalSubscriptionProductDTO>> GetAllOperatorSubscriptionProductAsync();
        Task<CustomerSubscriptionProductDTO?> DeleteOperatorSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionId);
        Task<CustomerSubscriptionProductDTO> UpdateOperatorSubscriptionProductForCustomerAsync(Guid organizationId, CustomerSubscriptionProductDTO subscriptionId);

    }
}
