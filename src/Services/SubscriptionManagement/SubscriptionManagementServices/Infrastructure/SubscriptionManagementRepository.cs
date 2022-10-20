using Common.Enums;
using Common.Extensions;
using Common.Logging;
using Common.Seedwork;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository<T> : ISubscriptionManagementRepository<T> where T: ISubscriptionOrder
    {
        private readonly SubscriptionManagementContext _subscriptionManagementContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;


        public SubscriptionManagementRepository(SubscriptionManagementContext subscriptionManagementContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _subscriptionManagementContext = subscriptionManagementContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            var numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionManagementContext).ExecuteAsync(async () =>
            {
                var editedEntities = _subscriptionManagementContext.ChangeTracker.Entries()
                    .Where(entity => entity.State == EntityState.Modified)
                    .ToList();

                editedEntities.ForEach(entity =>
                {
                    if (!entity.Entity.GetType().IsSubclassOf(typeof(ValueObject)))
                    {
                        entity.Property("LastUpdatedDate").CurrentValue = DateTime.UtcNow;
                    }
                });
                if (!_subscriptionManagementContext.IsSQLite)
                {
                    // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                    foreach (var @event in _subscriptionManagementContext.GetDomainEventsAsync())
                    {
                        await _functionalEventLogService.SaveEventAsync(@event, _subscriptionManagementContext.Database.CurrentTransaction);
                    }
                }
                numberOfRecordsSaved = await _subscriptionManagementContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_subscriptionManagementContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<List<ISubscriptionOrder>> GetAllSubscriptionOrdersForCustomer(Guid organizationId)
        {
            var orders = await _subscriptionManagementContext.TransferSubscriptionOrders.Include(o => o.PrivateSubscription)
                .Include(o => o.BusinessSubscription).Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            var subscriptionOrderList = orders.ToList();

            var transferToPrivateOrders = await _subscriptionManagementContext.TransferToPrivateSubscriptionOrders
                .Include(o => o.UserInfo).Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(transferToPrivateOrders);

            var changeOrders = await _subscriptionManagementContext.ChangeSubscriptionOrder
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(changeOrders);

            var cancelOrders = await _subscriptionManagementContext.CancelSubscriptionOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(cancelOrders);

            var orderSimSubscriptionOrders = await _subscriptionManagementContext.OrderSimSubscriptionOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(orderSimSubscriptionOrders);

            var activateSimOrders = await _subscriptionManagementContext.ActivateSimOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(activateSimOrders);

            var newSubscription = await _subscriptionManagementContext.NewSubscriptionOrders
                .Where(o => o.OrganizationId == organizationId)
                .ToListAsync<ISubscriptionOrder>();
            subscriptionOrderList.AddRange(newSubscription);

            return subscriptionOrderList.OrderByDescending(o=> o.CreatedDate).Take(15).ToList();
        }

        public async Task<int> GetTotalSubscriptionOrdersCountForCustomer(Guid organizationId, IList<SubscriptionOrderTypes>? orderTypes = null, string? phoneNumber = null, bool checkOrderExist = false)
        {
            int subscriptionsCount = 0;
            if (orderTypes == null || orderTypes.Count() == 0)
            {
                orderTypes = Enum.GetValues<SubscriptionOrderTypes>().ToList();
            }
            foreach (var orderType in orderTypes)
            {
                switch (orderType)
                {
                    case SubscriptionOrderTypes.TransferToPrivate:
                        var transferToPrivateOrdersCount = await _subscriptionManagementContext.TransferToPrivateSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber ))
                            .CountAsync();
                        subscriptionsCount += transferToPrivateOrdersCount;
                        break;
                    case SubscriptionOrderTypes.TransferToBusiness:
                        var transferToBusinessOrdersCount = await _subscriptionManagementContext.TransferSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += transferToBusinessOrdersCount;
                        break;
                    case SubscriptionOrderTypes.OrderSim:
                        if (string.IsNullOrEmpty(phoneNumber))
                        {
                            var orderSimSubscriptionOrdersCount = await _subscriptionManagementContext.OrderSimSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId)
                            .CountAsync();
                            subscriptionsCount += orderSimSubscriptionOrdersCount;
                        }
                        break;
                    case SubscriptionOrderTypes.ActivateSim:
                        var activateSimOrdersCount = await _subscriptionManagementContext.ActivateSimOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += activateSimOrdersCount;
                        break;
                    case SubscriptionOrderTypes.NewSubscription:
                        if (string.IsNullOrEmpty(phoneNumber))
                        {
                            var newSubscriptionCount = await _subscriptionManagementContext.NewSubscriptionOrders
                                        .Where(o => o.OrganizationId == organizationId)
                            .CountAsync();
                            subscriptionsCount += newSubscriptionCount;
                        }
                        
                        break;
                    case SubscriptionOrderTypes.ChangeSubscription:
                        var changeOrdersCount = await _subscriptionManagementContext.ChangeSubscriptionOrder
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += changeOrdersCount;
                        break;
                    case SubscriptionOrderTypes.CancelSubscription:
                        var cancelOrdersCount = await _subscriptionManagementContext.CancelSubscriptionOrders
                            .Where(o => o.OrganizationId == organizationId && (string.IsNullOrEmpty(phoneNumber) || o.MobileNumber == phoneNumber))
                            .CountAsync();
                        subscriptionsCount += cancelOrdersCount;
                        break;
                    default:
                        break;
                }

                if (checkOrderExist && subscriptionsCount > 0)
                    return subscriptionsCount;
            }
            return subscriptionsCount;
        }
        public async Task<T> AddSubscriptionOrder(T subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            // ReSharper disable once UnusedVariable
            var numberOfRecords = await SaveEntitiesAsync();
            return (T)added.Entity;
        }

        public async Task<OrderSimSubscriptionOrder> GetOrderSimOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.OrderSimSubscriptionOrders
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }
        public async Task<ActivateSimOrder> GetActivateSimOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.ActivateSimOrders
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<TransferToBusinessSubscriptionOrder> GetTransferToBusinessOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.TransferSubscriptionOrders
                .Include(p => p.PrivateSubscription).ThenInclude(p => p.RealOwner)
                .Include(b => b.BusinessSubscription)
                .Include(a => a.SubscriptionAddOnProducts)
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<TransferToPrivateSubscriptionOrder> GetTransferToPrivateOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.TransferToPrivateSubscriptionOrders
                .Include(p => p.UserInfo)
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }
        public async Task<NewSubscriptionOrder> GetNewSubscriptionOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.NewSubscriptionOrders
                .Include(p => p.PrivateSubscription)
                .Include(b => b.BusinessSubscription)
                .Include(a => a.SubscriptionAddOnProducts)
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<ChangeSubscriptionOrder> GetChangeSubscriptionOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.ChangeSubscriptionOrder
               .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);

            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }

        public async Task<CancelSubscriptionOrder> GetCancelSubscriptionOrder(Guid subscriptionOrder)
        {
            var order = await _subscriptionManagementContext.CancelSubscriptionOrders
                .FirstOrDefaultAsync(s => s.SubscriptionOrderId == subscriptionOrder);
            
            return order ?? throw new ArgumentException($"Can't find the order with id: {subscriptionOrder}");
        }
    }
}