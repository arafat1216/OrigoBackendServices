namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {
        Task<Operator?> GetOperatorAsync(int id);
        Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid customerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(SubscriptionProduct subscriptionProduct);
        Task<CustomerSubscriptionProduct?> GetAvailableSubscriptionProductForCustomerbySubscriptionIdAsync(Guid customerId, int subscriptionId);
        Task<IList<CustomerSubscriptionProduct>?> GetAllCustomerSubscriptionProductsAsync(Guid customerId);
        Task<IList<SubscriptionProduct>?> GetAllOperatorSubscriptionProducts();
        Task DeleteOperatorSubscriptionProductForCustomerAsync(CustomerSubscriptionProduct customerSubscriptionProduct);
        Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<SubscriptionOrder> AddSubscriptionOrderAsync(SubscriptionOrder subscriptionOrder);
        Task<TransferSubscriptionOrder> TransferSubscriptionOrderAsync(TransferSubscriptionOrder subscriptionOrder);
        Task<SubscriptionProduct?> GetSubscriptionProductAsync(int id);
        Task<DataPackage?> GetDataPackageAsync(int id);
        Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id);
        Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(Guid organizationId, string accountNumber);
        Task DeleteCustomerOperatorAccountAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId);
        Task<CustomerOperatorSettings> GetCustomerOperatorSettings(Guid customerId,string operatorName);
        Task<CustomerOperatorSettings> AddCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings);
        Task<CustomerOperatorSettings> UpdateCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings);
        Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName,int operatorId);
        Task<IList<Operator>> GetAllOperatorsAsync();
    }
}
