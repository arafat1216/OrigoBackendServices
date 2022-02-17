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

        public async Task AddOperatorsForCustomerAsync(Guid customerId, IList<int> operators)
        {
            await CustomerSettingsRepository.AddCustomerSettingsAsync(customerId, operators);
        }

        public async Task<IList<CustomerReferenceFieldDTO>> GetCustomerReferenceFieldsAsync(Guid customerId)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId)
        {
            await CustomerSettingsRepository.DeleteOperatorForCustomerAsync(organizationId, operatorId);
        }
    }
}
