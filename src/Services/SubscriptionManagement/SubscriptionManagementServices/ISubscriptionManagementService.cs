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
        Task<SubscriptionOrder> AddSubscriptionOrderForCustomerAsync(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId, string simCardNumber);
        Task<TransferSubscriptionOrder> TransferSubscriptionOrderAsync(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId, string simCardNumber, DateTime orderExecutionDate, int newOperatorAccountId);
        Task<IEnumerable<CustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task<CustomerSubscriptionProductDTO> AddOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string>? dataPackages, Guid callerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, string accountName, int operatorId, Guid CallerId);
        Task DeleteCustomerOperatorAccountAsync(Guid organizationId, string accountNumber);
        Task<IList<CustomerSubscriptionProductDTO>> GetAllCustomerSubscriptionProductsAsync(Guid customerId);
        Task<IList<CustomerSubscriptionProductDTO>> GetOperatorSubscriptionProductForSettingsAsync(Guid customerId, string operatorName);
        Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
    }
}
