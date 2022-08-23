using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
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
public class SaveContextChangesInterceptor : SaveChangesInterceptor
{
    /// <summary>
    ///     Gets or sets the caller's identifier.
    /// </summary>
    /// <value>
    ///     The caller's identifier.
    /// </value>
    private Guid? CallerId { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SaveContextChangesInterceptor"/> class, and assigns a global
    ///     callerID.
    /// </summary>
    /// <param name="callerId"> The caller's identifier. </param>
    public SaveContextChangesInterceptor(Guid? callerId)
    {
        CallerId = callerId;
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

        var timestamp = DateTimeOffset.UtcNow;

        foreach (var entry in eventData.Context.ChangeTracker.Entries())
        {
            // We only want to process auditable entities, so let's make sure it is one, and skip the rest
            if (entry.Entity is not Auditable auditableEntity)
            {
                continue;
            }

            switch (entry.State)
            {
                case EntityState.Modified:
                    HandleModified(ref auditableEntity, timestamp);
                    break;
                case EntityState.Added:
                    HandleAdded(ref auditableEntity, timestamp);
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
    ///     Set details for entities with <see cref="EntityState.Added"/>.
    /// </summary>
    /// <param name="auditableEntity"> The <see cref="Auditable"/> entity to be processed. </param>
    /// <param name="timestamp"> The creation timestamp. </param>
    private void HandleAdded<T>(ref T auditableEntity, in DateTimeOffset timestamp) where T : Auditable
    {
        // We should do a safety-check on the existing values in case someone has manually changed the tracking state to 'Added',
        // or manually assigned the values (e.g. in unit tests).
        if (auditableEntity.DateCreated == default || auditableEntity.DateCreated == null)
        {
            SetPrivateProperty(ref auditableEntity, nameof(auditableEntity.DateCreated), timestamp);
        }

        if (CallerId is not null && (auditableEntity.CreatedBy == default || auditableEntity.CreatedBy == null ||
                                     auditableEntity.CreatedBy == Guid.Empty))
        {
            SetPrivateProperty(ref auditableEntity, nameof(auditableEntity.CreatedBy), CallerId.Value);
        }

        // There should not be any updated by info in new entities (as it's never been modified).
        SetPrivateProperty<T, Guid?>(ref auditableEntity, nameof(auditableEntity.UpdatedBy), null);
        SetPrivateProperty<T, DateTimeOffset?>(ref auditableEntity, nameof(auditableEntity.DateUpdated), null);
    }


    /// <summary>
    ///     Updates the details for entities with <see cref="EntityState.Modified" />.
    /// </summary>
    /// <param name="auditableEntity"> The <see cref="Auditable"/> entity to be processed. </param>
    /// <param name="timestamp"> The creation timestamp. </param>
    private void HandleModified<T>(ref T auditableEntity, in DateTimeOffset timestamp) where T : Auditable
    {
        SetPrivateProperty(ref auditableEntity, nameof(auditableEntity.DateUpdated), timestamp);

        if (CallerId is not null)
        {
            SetPrivateProperty(ref auditableEntity, nameof(auditableEntity.UpdatedBy), CallerId);
        }
    }


    /// <summary>
    ///     Uses reflections to access and set a value in a private-property.
    /// </summary>
    /// <typeparam name="TEntity"> The class that is used in <paramref name="entity"/>. </typeparam>
    /// <typeparam name="TDatatype"> The datatype that is used in <paramref name="propertyName"/>. </typeparam>
    /// <param name="entity"> The entity we are using reflections on. </param>
    /// <param name="propertyName"> The '<c>nameof()</c>' for the private property that is accessed. </param>
    /// <param name="newValue"> The new value that will be set. </param>
    private void SetPrivateProperty<TEntity, TDatatype>(ref TEntity entity, string propertyName, TDatatype newValue)
    {
        var propertyInfo = typeof(TEntity).GetProperty(propertyName);

        if (propertyInfo == null)
        {
            return;
        }

        propertyInfo.SetValue(entity, newValue);
    }
}