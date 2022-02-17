using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;

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
        Task<IList<OrigoCustomerReferenceField>> GetAllCustomerReferenceFieldsAsync(Guid organizationId);
        Task<OrigoCustomerReferenceField> AddCustomerReferenceFieldAsync(Guid organizationId, NewCustomerReferenceField newCustomerReferenceField, string callerId);
        Task<OrigoCustomerReferenceField> DeleteCustomerReferenceFieldAsync(Guid organizationId, int customerReferenceId, string callerId);
    }
}