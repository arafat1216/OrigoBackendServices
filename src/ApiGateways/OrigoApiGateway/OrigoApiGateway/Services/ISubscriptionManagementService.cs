using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrigoApiGateway.Services
{
    public interface ISubscriptionManagementService
    {
        Task<IEnumerable<OrigoCustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task AddOperatorAccountForCustomerAsync(Guid organizationId, OrigoCustomerOperatorAccount origoCustomerOperatorAccount);
        Task DeleteOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber);
        Task<OrigoOperator> GetOperatorAsync(int id);
        Task<IList<OrigoOperator>> GetAllOperatorsAsync();
        Task<IList<OrigoOperator>> GetAllOperatorsForCustomerAsync(Guid organizationId);
        Task AddOperatorForCustomerAsync(Guid organizationId, IList<int> operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
        Task<bool> AddSubscriptionForCustomerAsync(Guid organizationId, OrderTransferSubscription subscription);
        Task TransferSubscriptionOrderForCustomerAsync(Guid customerId, TransferSubscriptionOrder order);
        Task<OrigoSubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid organizationId, NewSubscriptionProduct subscriptionProduct);
        Task<IList<OrigoSubscriptionProduct>> GetSubscriptionProductForCustomerAsync(Guid organizationId, string operatorName);
        Task<OrigoSubscriptionProduct> DeleteSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionProductId);
        Task<OrigoSubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionProductId, UpdateSubscriptionProduct subscriptionProduct);
    }
}