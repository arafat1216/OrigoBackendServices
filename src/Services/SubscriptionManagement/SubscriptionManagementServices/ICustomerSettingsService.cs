using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ICustomerSettingsService
    {
        Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid customerId);
    }
}
