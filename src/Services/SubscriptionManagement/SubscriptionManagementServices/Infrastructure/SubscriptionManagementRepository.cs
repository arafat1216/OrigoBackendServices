using Common.Extensions;
using Common.Logging;
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
                    entity.Property("LastUpdatedDate").CurrentValue = DateTime.UtcNow;
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

            return subscriptionOrderList.OrderByDescending(o=> o.CreatedDate).Take(15).ToList();
        }

        public async Task<T> AddSubscriptionOrder(T subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            // ReSharper disable once UnusedVariable
            var numberOfRecords = await SaveEntitiesAsync();
            return (T)added.Entity;
        }
    }
}