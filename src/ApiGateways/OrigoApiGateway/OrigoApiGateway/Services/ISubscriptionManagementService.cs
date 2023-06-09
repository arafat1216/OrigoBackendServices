﻿using Common.Enums;
using Common.Interfaces;
using OrigoApiGateway.Models;
using OrigoApiGateway.Models.SubscriptionManagement;
using OrigoApiGateway.Models.SubscriptionManagement.Backend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Request;
using OrigoApiGateway.Models.SubscriptionManagement.Frontend.Response;

namespace OrigoApiGateway.Services
{
    public interface ISubscriptionManagementService
    {
        Task<IEnumerable<OrigoCustomerOperatorAccount>> GetAllOperatorAccountsForCustomerAsync(Guid customerId);
        Task<OrigoCustomerOperatorAccount> AddOperatorAccountForCustomerAsync(Guid organizationId, NewOperatorAccount newCustomerOperatorAccount);
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
        Task<IList<OrigoSubscriptionProduct>> GetAllSubscriptionProductForCustomerAsync(Guid organizationId, bool includeOperator = false);
        Task<OrigoSubscriptionProduct> DeleteSubscriptionProductForCustomerAsync(Guid organizationId, int subscriptionProductId);
        Task<OrigoSubscriptionProduct> UpdateOperatorSubscriptionProductForCustomerAsync(Guid customerId, int subscriptionProductId, UpdateSubscriptionProduct subscriptionProduct);
        Task<IList<OrigoCustomerReferenceField>> GetAllCustomerReferenceFieldsAsync(Guid organizationId);
        Task<OrigoCustomerReferenceField> AddCustomerReferenceFieldAsync(Guid organizationId, NewCustomerReferenceField newCustomerReferenceField, string callerId);
        Task<OrigoCustomerReferenceField> DeleteCustomerReferenceFieldAsync(Guid organizationId, int customerReferenceId, string callerId);
        Task<OrigoTransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrderForCustomerAsync(Guid organizationId,
            TransferToPrivateSubscriptionOrder order, Guid callerId);
        Task<OrigoChangeSubscriptionOrder> ChangeSubscriptionOrderAsync(Guid organizationId, ChangeSubscriptionOrderPostRequest subscriptionOrderModel);
        Task<IList<OrigoSubscriptionOrderListItem>> GetSubscriptionOrders(Guid organizationId);
        Task<PagedModel<OrigoSubscriptionOrderListItem>> GetAllSubscriptionOrders(Guid organizationId, CancellationToken cancellationToken, FilterOptionsForSubscriptionOrder? filterOptions, string? search = null, int page = 1, int limit = 25);
        Task<int> GetSubscriptionOrdersCount(Guid organizationId, IList<SubscriptionOrderTypes>? orderTypes = null, string? phoneNumber = null, bool checkOrderExist = false);
        Task<OrigoCancelSubscriptionOrder> CancelSubscriptionOrderForCustomerAsync(Guid organizationId, CancelSubscriptionOrderDTO order);
        Task<OrigoOrderSim> OrderSimCardForCustomerAsync(Guid organizationId, OrderSim order, Guid callerId);
        Task<OrigoActivateSimOrder> ActivateSimCardForCustomerAsync(Guid organizationId, ActivateSimOrderPostRequest activateSimOrder);
        Task<OrigoNewSubscriptionOrder> NewSubscriptionOrder(Guid organizationId, NewSubscriptionOrderPostRequest requestModel);
        Task<IList<OrigoCustomerStandardPrivateSubscriptionProduct>> GetCustomerStandardPrivateSubscriptionProductAsync(Guid organizationId);
        Task<OrigoCustomerStandardPrivateSubscriptionProduct> PostCustomerStandardPrivateSubscriptionProductAsync(Guid organizationId, NewStandardPrivateProductDTO standardPrivateProduct);

        Task<OrigoCustomerStandardPrivateSubscriptionProduct> DeleteCustomerStandardPrivateSubscriptionProductAsync(Guid organizationId, int operatorId, Guid callerId);
        Task<IList<OrigoCustomerStandardBusinessSubscriptionProduct>> GetCustomerStandardBusinessSubscriptionProductAsync(Guid organizationId);
        Task<OrigoCustomerStandardBusinessSubscriptionProduct> PostCustomerStandardBusinessSubscriptionProductAsync(Guid organizationId, NewStandardBusinessSubscriptionProduct standardBusinessProduct);

        Task<OrigoCustomerStandardBusinessSubscriptionProduct> DeleteCustomerStandardBusinessSubscriptionProductAsync(Guid organizationId, int operatorId);
        Task<OrigoSubscriptionOrderDetailView> GetSubscriptionOrderDetailViewAsync(Guid organizationId, Guid orderId, int orderType);


    }
}