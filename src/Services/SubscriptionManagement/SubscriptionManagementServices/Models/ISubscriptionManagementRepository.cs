namespace SubscriptionManagementServices.Models
{
    public interface ISubscriptionManagementRepository
    {


        Task<IEnumerable<CustomerOperatorAccount>> GetAllCustomerOperatorAccountsAsync(Guid customerId);
        Task<CustomerOperatorAccount> AddOperatorAccountForCustomerAsync(CustomerOperatorAccount customerOperatorAccount);
        Task<Operator> GetOperatorAsync(string name);
    }
}
