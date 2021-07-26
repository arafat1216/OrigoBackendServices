using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Logging
{
    /// <summary>
    /// Handles functional log entries. Taken from https://github.com/dotnet-architecture/eShopOnContainers
    /// </summary>
    public class FunctionalEventLogService : IFunctionalEventLogService, IDisposable
    {
        private readonly FunctionalEventLogContext _functionalEventLogContext;
        private readonly DbConnection _dbConnection;
        private readonly List<Type> _eventTypes;
        private volatile bool disposedValue;

        public FunctionalEventLogService(DbConnection dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));
            _functionalEventLogContext = new FunctionalEventLogContext(
                new DbContextOptionsBuilder<FunctionalEventLogContext>()
                    .UseSqlServer(_dbConnection)
                    .Options);

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly()?.FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(BaseEvent)))
                .ToList();
        }

        public async Task<IEnumerable<FunctionalEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
        {
            var tid = transactionId.ToString();

            var result = await _functionalEventLogContext.FunctionalEventLogs
                .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished).ToListAsync();

            if (result != null && result.Any())
            {
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<FunctionalEventLogEntry>();
        }

        public Task SaveEventAsync(BaseEvent @event, IDbContextTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException(nameof(transaction));

            var eventLogEntry = new FunctionalEventLogEntry(@event, transaction.TransactionId);

            _functionalEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _functionalEventLogContext.FunctionalEventLogs.Add(eventLogEntry);

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
            var eventLogEntry = _functionalEventLogContext.FunctionalEventLogs.Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _functionalEventLogContext.FunctionalEventLogs.Update(eventLogEntry);

            return _functionalEventLogContext.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _functionalEventLogContext?.Dispose();
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
