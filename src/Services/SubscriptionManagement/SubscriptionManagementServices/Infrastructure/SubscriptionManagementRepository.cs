using Common.Extensions;
using Common.Logging;
using Common.Utilities;
using MediatR;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure
{
    public class SubscriptionManagementRepository : ISubscriptionManagementRepository
    {
        private readonly SubscriptionManagementContext _subscriptionContext;
        private readonly IFunctionalEventLogService _functionalEventLogService;
        private readonly IMediator _mediator;


        public SubscriptionManagementRepository(SubscriptionManagementContext subscriptionContext, IFunctionalEventLogService functionalEventLogService, IMediator mediator)
        {
            _subscriptionContext = subscriptionContext;
            _functionalEventLogService = functionalEventLogService;
            _mediator = mediator;
        }

        public async Task<TransferToBusinessSubscriptionOrder> TransferToBusinessSubscriptionOrderAsync(TransferToBusinessSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await SaveEntitiesAsync();
            return added.Entity;
        }


        private async Task<int> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var numberOfRecordsSaved = 0;
            //Use of an EF Core resiliency strategy when using multiple DbContexts within an explicit BeginTransaction():
            //See: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency            
            await ResilientTransaction.New(_subscriptionContext).ExecuteAsync(async () =>
            {
                // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                foreach (var @event in _subscriptionContext.GetDomainEventsAsync())
                {
                    await _functionalEventLogService.SaveEventAsync(@event, _subscriptionContext.Database.CurrentTransaction);
                }
                await _subscriptionContext.SaveChangesAsync(cancellationToken);
                numberOfRecordsSaved = await _subscriptionContext.SaveChangesAsync(cancellationToken);
                await _mediator.DispatchDomainEventsAsync(_subscriptionContext);
            });
            return numberOfRecordsSaved;
        }

        public async Task<TransferToPrivateSubscriptionOrder> TransferToPrivateSubscriptionOrderAsync(TransferToPrivateSubscriptionOrder subscriptionOrder)
        {
            var added = await _subscriptionContext.AddAsync(subscriptionOrder);
            await SaveEntitiesAsync();
            return added.Entity;
        }
    }
}