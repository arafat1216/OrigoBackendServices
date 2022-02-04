namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {
        Task<Operator?> GetOperatorAsync(int id);
        Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid customerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<Operator> GetOperatorAsync(string name);
        Task<SubscriptionOrder> AddSubscriptionOrderAsync(SubscriptionOrder subscriptionOrder);
        Task<SubscriptionProduct?> GetSubscriptionProductAsync(int id);
        Task<Datapackage?> GetDatapackageAsync(int id);
        Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id);
        Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId);
    }
}
