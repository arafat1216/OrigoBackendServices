namespace SubscriptionManagementServices.Models
{
    public interface ICustomerSettingsRepository
    {
        Task AddCustomerSettingsAsync(Guid customerId, IList<int> operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
    }
}
