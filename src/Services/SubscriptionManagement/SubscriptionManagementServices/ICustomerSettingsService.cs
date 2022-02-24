using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ICustomerSettingsService
    {
        Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid customerId);
        Task<CustomerReferenceFieldDTO> AddCustomerReferenceFieldAsync(Guid organizationId, string name, string type, Guid callerId);

        Task AddOperatorsForCustomerAsync(Guid customerId, IList<int> operators, Guid callerId);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
        Task<CustomerReferenceFieldDTO?> DeleteCustomerReferenceFieldsAsync(Guid organizationId, int customerReferenceFieldId);
    }
}
