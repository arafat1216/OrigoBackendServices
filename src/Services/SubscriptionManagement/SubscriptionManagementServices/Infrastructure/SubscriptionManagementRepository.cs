using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository : ISubscriptionManagementRepository
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

        public async Task<TransferToBusinessSubscriptionOrder> TransferToBusinessSubscriptionOrderAsync(TransferToBusinessSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            await SaveEntitiesAsync();
            return added.Entity;
        }

        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionManagementContext).ExecuteAsync(async () =>
            {
                var editedEntities = _subscriptionManagementContext.ChangeTracker.Entries()
                    .Where(E => E.State == EntityState.Modified)
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

        public async Task<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrderAsync(TransferToPrivateSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            await SaveEntitiesAsync();
            return added.Entity;
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

            return subscriptionOrderList.OrderByDescending(o=> o.CreatedDate).ToList();
        }

        public async Task<ChangeSubscriptionOrder> AddChangeSubscriptionOrderAsync(ChangeSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            await SaveEntitiesAsync();
            return added.Entity;
        }

        public async Task<CancelSubscriptionOrder> AddCancelSubscriptionOrderAsync(CancelSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionManagementContext.AddAsync(subscriptionOrder);
            await SaveEntitiesAsync();
            return added.Entity;
        }
    }
}