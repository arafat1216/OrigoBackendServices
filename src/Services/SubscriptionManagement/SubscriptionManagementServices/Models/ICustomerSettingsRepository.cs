namespace SubscriptionManagementServices.Models
{
    public interface ICustomerSettingsRepository
    {
        Task<CustomerSettings?> GetCustomerSettingsAsync(Guid customerId);
        Task<CustomerSettings> AddCustomerSettingsAsync(CustomerSettings customerSettings);
        Task<CustomerSettings> UpdateCustomerSettingsAsync(CustomerSettings customerSettings);
        Task AddCustomerOperatorSettingsAsync(Guid customerId, IList<int> operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
        Task<IReadOnlyCollection<CustomerReferenceField>> GetCustomerReferenceFieldsAsync(Guid organizationId);
        Task DeleteCustomerReferenceFieldForCustomerAsync(CustomerReferenceField customerReferenceField);
    }
}
