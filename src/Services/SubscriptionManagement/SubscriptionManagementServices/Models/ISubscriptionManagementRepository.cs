namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {
        Task<Operator?> GetOperatorAsync(int id);
        Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid customerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(SubscriptionProduct subscriptionProduct);
        Task<IList<SubscriptionProduct>?> GetOperatorSubscriptionProductForCustomerAsync(Guid customerId, string operatorName);
        Task<IList<SubscriptionProduct>?> GetSubscriptionProductForOperatorAsync(string operatorName);
        Task<SubscriptionProduct> DeleteOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<SubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionId);
        Task<Operator?> GetOperatorAsync(string name);
        Task<SubscriptionOrder> AddSubscriptionOrderAsync(SubscriptionOrder subscriptionOrder);
        Task<TransferSubscriptionOrder> TransferSubscriptionOrderAsync(TransferSubscriptionOrder subscriptionOrder);
        Task<SubscriptionProduct?> GetSubscriptionProductAsync(int id);
        Task<DataPackage?> GetDatapackageAsync(int id);
        Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(int id);
        Task<CustomerOperatorAccount?> GetCustomerOperatorAccountAsync(Guid organizationId, string accountNumber);
        Task DeleteCustomerOperatorAccountAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<IList<Operator>> GetAllOperatorsForCustomerAsync(Guid customerId);
        Task<CustomerOperatorSettings> GetCustomerOperatorSettings(Guid customerId,string operatorName);
        Task<CustomerOperatorSettings> AddCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings);
        Task<CustomerOperatorSettings> UpdateCustomerOperatorSettingsAsync(CustomerOperatorSettings customerOperatorSettings);
        Task <CustomerSettings> GetCustomerSettingsAsync(Guid customerId);
        Task<CustomerSettings> AddCustomerSettingsAsync(CustomerSettings customerSettings);
        Task<CustomerSettings> UpdateCustomerSettingsAsync(CustomerSettings customersettings);
        Task<SubscriptionProduct?> GetSubscriptionProductByNameAsync(string subscriptionProductName,int operatorId);
        Task<IList<Operator>> GetAllOperatorsAsync();
    }
}
