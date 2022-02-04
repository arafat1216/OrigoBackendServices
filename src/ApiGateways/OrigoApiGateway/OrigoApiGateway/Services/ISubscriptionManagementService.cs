using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ISubscriptionManagementService
    {
        Task<OrigoOperator> GetOperator(string operatorName);
        Task<IList<string>> GetAllOperators();
        Task<IList<string>> GetAllOperatorsForCustomer(Guid organizationId);
        Task<bool> AddOperatorForCustomerAsync(Guid organizationId, IList<string> operators);
        Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName);
        Task<bool> AddSubscriptionForCustomerAsync(Guid organizationId, OrderTransferSubscription subscription);
        Task<OrigoSubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid organizationId, NewSubscriptionProduct subscriptionProduct);
        Task<IList<OrigoSubscriptionProduct>> GetSubscriptionProductForCustomerAsync(Guid organizationId, string operatorName);
    }
}