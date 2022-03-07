using OrigoApiGateway.Models.SubscriptionManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;

namespace OrigoApiGateway.Services
{
    public interface ISubscriptionManagementService
    {
        Task<IEnumerable<OrigoCustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task<OrigoCustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid organizationId, NewOperatorAccount origoCustomerOperatorAccount);
        Task DeleteOperatorAccountForCustomerAsync(Guid organizationId, string accountNumber, int operatorId);
        Task<OrigoOperator> GetOperatorAsync(int id);
        Task<IList<OrigoOperator>> GetAllOperatorsAsync();
        Task<IList<OrigoOperator>> GetAllOperatorsForCustomerAsync(Guid organizationId);
        Task AddOperatorForCustomerAsync(Guid organizationId, NewOperatorListDTO operators);
        Task DeleteOperatorForCustomerAsync(Guid organizationId, int operatorId);
        Task<bool> AddSubscriptionForCustomerAsync(Guid organizationId, OrderTransferSubscription subscription);
        Task<OrigoTransferToBusinessSubscriptionOrder> TransferToBusinessSubscriptionOrderForCustomerAsync(
            Guid customerId, TransferToBusinessSubscriptionOrder order, Guid callerId);
        Task<IList<OrigoSubscriptionProduct>> GetAllOperatorsSubscriptionProductsAsync();
        Task<OrigoSubscriptionProduct> AddSubscriptionProductForCustomerAsync(Guid organizationId, NewSubscriptionProduct subscriptionProduct);
        Task<IList<OrigoSubscriptionProduct>> GetAllSubscriptionProductForCustomerAsync(Guid organizationId);
        Task<OrigoSubscriptionProduct> DeleteSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionProductId);
        Task<OrigoSubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionProductId, UpdateSubscriptionProduct subscriptionProduct);
        Task<IList<OrigoCustomerReferenceField>> GetAllCustomerReferenceFieldsAsync(Guid organizationId);
        Task<OrigoCustomerReferenceField> AddCustomerReferenceFieldAsync(Guid organizationId, NewCustomerReferenceField newCustomerReferenceField, string callerId);
        Task<OrigoCustomerReferenceField> DeleteCustomerReferenceFieldAsync(Guid organizationId, int customerReferenceId, string callerId);
        Task<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrderForCustomerAsync(Guid organizationId,
            TransferToPrivateSubscriptionOrder order, Guid callerId);
        Task<OrigoChangeSubscriptionOrder> ChangeSubscriptionOrderAsync(Guid organizationId, ChangeSubscriptionOrderPostRequest subscriptionOrderModel);
        Task<IList<OrigoSubscriptionOrderListItem>> GetSubscriptionOrders(Guid organizationId);
        Task<OrigoCancelSubscriptionOrder> CancelSubscriptionOrderForCustomerAsync(Guid organizationId, CancelSubscriptionOrder order, Guid callerId);
        Task<OrigoOrderSim> OrderSimCardForCustomerAsync(Guid organizationId, OrderSim order, Guid callerId);
    }
}