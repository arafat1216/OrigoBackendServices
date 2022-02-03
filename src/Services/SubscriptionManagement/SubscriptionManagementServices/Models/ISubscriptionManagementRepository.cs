namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {
        Task<Operator?> GetOperatorAsync(int id);
        Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid customerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> datapackages);
        Task<IList<SubscriptionProduct>> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName);
        Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<Operator> GetOperatorAsync(string name);
    }
}
