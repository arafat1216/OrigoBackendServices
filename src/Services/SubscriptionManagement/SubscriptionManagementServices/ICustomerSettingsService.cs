using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ICustomerSettingsService
    {
        Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid customerId);
        Task AddOperatorsForCustomerAsync(Guid customerId, IList<int> operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
    }
}
