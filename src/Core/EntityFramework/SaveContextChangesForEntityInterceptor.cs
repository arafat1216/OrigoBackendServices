using Common.Exceptions;
using Common.Infrastructure;
using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Common.EntityFramework;

/// <summary>
///     An interceptor for Entity Framework context-classes. The interceptor must be registered in the context before it is
///     applied.
///     <para>
///         The interceptor overrides the save methods, and automatically assigns the caller ID, along with corresponding
///         timestamps
///         (e.g. "UpdatedBy", "DateUpdated").
///     </para>
/// </summary>
public class SaveContextChangesForEntityInterceptor : SaveChangesInterceptor
{
    private readonly IApiRequesterService _apiRequesterService;

    /// <summary>
    ///     Gets or sets the caller's identifier.
    /// </summary>
    /// <value>
    ///     The caller's identifier.
    /// </value>
    private Guid? CallerId => _apiRequesterService.AuthenticatedUserId;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SaveContextChangesInterceptor" /> class, and assigns a global
    ///     callerID.
    /// </summary>
    /// <param name="apiRequesterService">The service to get the id of the authenticated user from</param>
    public SaveContextChangesForEntityInterceptor(IApiRequesterService apiRequesterService)
    {
        _apiRequesterService = apiRequesterService;
    }

    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditProperties(ref eventData);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
        InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SetAuditProperties(ref eventData);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void SetAuditProperties(ref DbContextEventData eventData)
    {
        if (eventData.Context is null)
        {
            throw new InvalidOperationException("DbContext is null!");
        }

        var timestamp = DateTime.UtcNow;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            // We only want to process auditable entities, so let's make sure it is one, and skip the rest
            if (entry.Entity is not Entity auditableEntity)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Modified:
                    HandleModified(auditableEntity, timestamp, eventData.Context);
                    break;
                case EntityState.Added:
                    HandleAdded(auditableEntity, timestamp, eventData.Context);
                    break;
                case EntityState.Deleted:
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    break;
            }
        }
    }

    /// <summary>
    ///     Set details for entities with <see cref="EntityState.Added" />.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="timestamp"> The creation timestamp. </param>
    /// <param name="dbContext"></param>
    private void HandleAdded(Entity entity, in DateTime timestamp, DbContext dbContext)
    {
        SetProperty(dbContext.Entry(entity).Property(e => e.CreatedDate), timestamp);
        SetProperty(dbContext.Entry(entity).Property(e => e.LastUpdatedDate), timestamp);
        if (CallerId is null) return;
        SetProperty(dbContext.Entry(entity).Property(e => e.CreatedBy), CallerId.Value);
    }

    /// <summary>
    ///     Updates the details for entities with <see cref="EntityState.Modified" />.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="timestamp"> The creation timestamp. </param>
    /// <param name="dbContext"></param>
    private void HandleModified(Entity entity, in DateTime timestamp, DbContext dbContext)
    {
        SetProperty(dbContext.Entry(entity).Property(e => e.LastUpdatedDate), timestamp);
        if (CallerId is null) return;
        SetProperty(dbContext.Entry(entity).Property(e => e.UpdatedBy), CallerId.Value);
    }

    /// <summary>
    ///     Sets value for property entry and marks as modified.
    /// </summary>
    /// <typeparam name="TDataType"> The data type that is used in <paramref name="propertyName" />. </typeparam>
    /// <param name="propertyEntry">The property entry to be updated</param>
    /// <param name="newValue"> The new value that will be set. </param>
    private void SetProperty<TDataType>(PropertyEntry<Entity, TDataType> propertyEntry, TDataType newValue)
    {
        propertyEntry.CurrentValue = newValue;
        propertyEntry.IsModified = true;
    }
}