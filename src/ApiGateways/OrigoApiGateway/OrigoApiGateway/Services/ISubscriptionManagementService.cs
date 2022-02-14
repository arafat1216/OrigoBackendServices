using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ISubscriptionManagementService
    {
        Task<IEnumerable<OrigoCustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task AddOperatorAccountForCustomerAsync(Guid customerId, OrigoCustomerOperatorAccount origoCustomerOperatorAccount);
        Task<OrigoOperator> GetOperator(string operatorName);
        Task<IList<string>> GetAllOperators();
        Task<IList<OrigoOperator>> GetAllOperatorsForCustomerAsync(Guid organizationId);
        Task<bool> AddOperatorForCustomerAsync(Guid organizationId, IList<string> operators);
        Task<bool> DeleteOperatorForCustomerAsync(Guid organizationId, string operatorName);
        Task<bool> AddSubscriptionForCustomerAsync(Guid organizationId, OrderTransferSubscription subscription);
        Task TransferSubscriptionOrderForCustomerAsync(Guid customerId, TransferSubscriptionOrder order);
        Task<OrigoSubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid organizationId, NewSubscriptionProduct subscriptionProduct);
        Task<IList<OrigoSubscriptionProduct>> GetSubscriptionProductForCustomerAsync(Guid organizationId, string operatorName);
        Task<OrigoSubscriptionProduct> DeleteSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionProductId);
        Task<OrigoSubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionProductId, UpdateSubscriptionProduct subscriptionProduct);
    }
}