using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<IList<string>> GetAllOperatorsAsync();
        Task<Operator> GetOperator(string operatorName);
        Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId);
        Task<bool> AddOperatorForCustomerAsync(Guid customerId, IList<string> operators);
        Task<bool> DeleteOperatorForCustomerAsync(Guid customerId, string operatorName);
        Task<SubscriptionOrder> AddSubscriptionOrderForCustomerAsync(Guid customerId, int subscriptionProductId, int operatorAccountId, int datapackageId, Guid callerId);
        Task<IEnumerable<CustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid customerId, Guid organizationId, string accountNumber, string accountName, int operatorId, Guid CallerId);
        Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> Datapackages, Guid callerId);
        Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName);
        Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
    }
}
