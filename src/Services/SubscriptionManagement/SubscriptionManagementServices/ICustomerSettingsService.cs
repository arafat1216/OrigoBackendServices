using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ICustomerSettingsService
    {
        Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid customerId);
        Task<CustomerReferenceFieldDTO> AddCustomerReferenceFieldAsync(Guid organizationId, string name, string type, Guid callerId);

        Task AddOperatorsForCustomerAsync(Guid customerId, IList<int> operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
    }
}
