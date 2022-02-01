using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices
{
    public interface ISubscriptionManagementService
    {
        Task<IList<string>> GetAllOperators();
        Task<IList<string>> GetOperator(string operatorName);
        Task<IList<string>> GetAllOperatorsForCustomer(Guid customerId);
        Task<bool> AddOperatorForCustomerAsync(Guid customerId, IList<string> operators);
        Task<bool> DeleteOperatorForCustomerAsync(Guid customerId, string operatorName);
        Task<bool> AddSubscriptionForCustomerAsync(Guid customerId);
        Task<SubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid customerId, string operatorName, string productName, IList<string> Datapackages);
    }
}
