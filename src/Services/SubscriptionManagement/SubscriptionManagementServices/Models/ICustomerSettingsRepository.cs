namespace SubscriptionManagementServices.Models
{
    public interface ICustomerSettingsRepository
    {
        Task<CustomerSettings?> GetCustomerSettingsAsync(Guid organizationId, bool asNoTracking, bool includeCustomerOperatorSettings = false, bool includeOperator = false,
                    bool includeStandardPrivateSubscriptionProduct = false, bool includeCustomerOperatorAccounts = false, bool includeAvailableSubscriptionProducts = false,
                    bool includeGlobalSubscriptionProduct = false, bool includeDataPackages = false, bool includeCustomerReferenceFields = false);
        Task<CustomerSettings> AddCustomerSettingsAsync(CustomerSettings customerSettings);
        Task<CustomerSettings> UpdateCustomerSettingsAsync(CustomerSettings customerSettings);
        Task AddCustomerOperatorSettingsAsync(Guid customerId, IList<int> operators, Guid callerId);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
        Task<IReadOnlyCollection<CustomerReferenceField>> GetCustomerReferenceFieldsAsync(Guid organizationId, bool asNoTracking);
        Task DeleteCustomerReferenceFieldForCustomerAsync(CustomerReferenceField customerReferenceField);
        Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName, int operatorId);
        Task<IList<SubscriptionProduct>?> GetAllOperatorSubscriptionProducts(bool asNoTracking);
    }
}
