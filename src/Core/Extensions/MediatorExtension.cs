using Common.Seedwork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Common.Extensions
{
    public static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEntitiesList = domainEntities.ToList();
            var domainEvents = domainEntitiesList.SelectMany(x => x.Entity.DomainEvents).ToList();
            domainEntitiesList.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            foreach (var domainEvent in domainEvents) await mediator.Publish(domainEvent);
        }

        public static IList<INotification> GetDomainEventsAsync(this DbContext ctx)
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEntitiesList = domainEntities.ToList();
            return domainEntitiesList.SelectMany(x => x.Entity.DomainEvents).ToList();
        }
    }
}