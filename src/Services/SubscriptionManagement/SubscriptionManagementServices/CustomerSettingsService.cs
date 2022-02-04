using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public class CustomerSettingsService : ICustomerSettingsService
    {
        public CustomerSettingsService(ICustomerSettingsRepository customerSettingsRepository)
        {
            CustomerSettingsRepository = customerSettingsRepository;
        }

        public ICustomerSettingsRepository CustomerSettingsRepository { get; }

        public Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }
    }
}
