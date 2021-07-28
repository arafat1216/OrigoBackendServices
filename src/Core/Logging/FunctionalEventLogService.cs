using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Logging
{
    /// <summary>
    /// Handles functional log entries. Taken from https://github.com/dotnet-architecture/eShopOnContainers
    /// </summary>
    public sealed class FunctionalEventLogService : IFunctionalEventLogService, IDisposable
    {
        private readonly LoggingDbContext _functionalEventLogContext;
        private readonly List<Type> _eventTypes;
        private volatile bool _disposedValue;

        public FunctionalEventLogService(LoggingDbContext functionalEventLogContext)
        {
            if (functionalEventLogContext == null)
            {
                throw new ArgumentNullException(nameof(functionalEventLogContext));
            }

            _functionalEventLogContext = functionalEventLogContext;

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly()?.FullName ?? string.Empty)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(BaseEvent)))
                .ToList();
        }

        public async Task<IEnumerable<FunctionalEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            var result = await _functionalEventLogContext.LogEntries
                .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished).ToListAsync();

            if (result != null && result.Any())
            {
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<FunctionalEventLogEntry>();
        }

        public Task SaveEventAsync(INotification @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new FunctionalEventLogEntry(@event as IEvent, transaction.TransactionId);
            //_functionalEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _functionalEventLogContext.LogEntries.Add(eventLogEntry);

            return _functionalEventLogContext.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _functionalEventLogContext.LogEntries.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _functionalEventLogContext.LogEntries.Update(eventLogEntry);

            return _functionalEventLogContext.SaveChangesAsync();
        }

        private void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _functionalEventLogContext?.Dispose();
                }


                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
