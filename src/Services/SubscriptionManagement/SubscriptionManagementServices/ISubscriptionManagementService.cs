using SubscriptionManagementServices.Models;
using SubscriptionManagementServices.ServiceModels;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<Operator?> GetOperatorAsync(int id);
        Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId);
        Task<IList<Operator>> GetAllOperatorsAsync();
        Task<bool> DeleteOperatorForCustomerAsync(Guid customerId, string operatorName);
        //Task<SubscriptionOrder> AddSubscriptionOrderForCustomerAsync(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId, string simCardNumber);
        Task<PrivateToBusinessSubscriptionOrder> TransferPrivateToBusinessSubscriptionOrderAsync(Guid organizationId, PrivateToBusinessSubscriptionOrderDTO order);
        Task<IEnumerable<CustomerOperatorAccountDTO>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task<CustomerSubscriptionProductDTO> AddOperatorSubscriptionProductForCustomerAsync(Guid customerId, int operatorId, string productName, IList<string>? dataPackages, Guid callerId);
        Task<CustomerOperatorAccountDTO> AddOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, string accountName, int operatorId, Guid CallerId);
        Task DeleteCustomerOperatorAccountAsync(Guid organizationId, string accountNumber);
        Task<IList<CustomerSubscriptionProductDTO>> GetAllCustomerSubscriptionProductsAsync(Guid customerId);
        Task<IList<GlobalSubscriptionProductDTO>> GetAllOperatorSubscriptionProductAsync();
        Task<CustomerSubscriptionProductDTO> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
    }
}
