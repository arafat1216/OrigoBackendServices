using MediatR;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Logging
{
    public interface IFunctionalEventLogService
    {
        Task SaveEventAsync(INotification @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
        Task<IList<FunctionalEventLogEntry>> RetrieveEventLogsAsync(Guid entityId);
    }
}