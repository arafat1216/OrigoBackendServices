using Common.Seedwork;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Reflection;

namespace Common.EntityFramework
{
    /// <summary>
    ///     An interceptor for Entity Framework context-classes. The interceptor must be registered in the context before it is applied. <para>
    ///     
    ///     The interceptor overrides the save methods, and automatically assigns the caller ID, along with corresponding timestamps
    ///     (e.g. "UpdatedBy", "DateUpdated"). </para>
    /// </summary>
    public class SaveContextChangesInterceptor : SaveChangesInterceptor
    {
        /// <summary>
        ///     Gets or sets the caller's identifier.
        /// </summary>
        /// <value>
        ///     The caller's identifier.
        /// </value>
        private Guid? CallerId { get; set; }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SaveContextChangesInterceptor"/> class, without setting a callerID.
        /// </summary>
        public SaveContextChangesInterceptor() : base()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="SaveContextChangesInterceptor"/> class, and assigns a global callerID.
        /// </summary>
        /// <param name="callerId"> The caller's identifier. </param>
        public SaveContextChangesInterceptor(Guid? callerId) : base()
        {
            CallerId = callerId;
        }


        /// <inheritdoc/>
        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            SetAuditProperties(ref eventData);
            return base.SavingChanges(eventData, result);
        }


        /// <inheritdoc/>
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            SetAuditProperties(ref eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }



        private void SetAuditProperties(ref DbContextEventData eventData)
        {
            if (eventData.Context is null)
                throw new InvalidOperationException("DbContext is null!");

            DateTimeOffset timestamp = DateTimeOffset.UtcNow;

            foreach (var entry in eventData.Context.ChangeTracker.Entries())
            {
                var auditableEntity = entry.Entity as Auditable;

                // We only want to process auditable entities
                if (auditableEntity is null)
                    continue;

                switch (entry.State)
                {
                    case Microsoft.EntityFrameworkCore.EntityState.Detached:
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Unchanged:
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Deleted:
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Modified:
                        HandleModified(auditableEntity, timestamp);
                        break;
                    case Microsoft.EntityFrameworkCore.EntityState.Added:
                        HandleAdded(auditableEntity, timestamp);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        ///     Set details for entities with <see cref="Microsoft.EntityFrameworkCore.EntityState.Added"/>.
        /// </summary>
        /// <param name="auditable"> The <see cref="Auditable"/> entity to be processed. </param>
        /// <param name="timestamp"> The creation timestamp. </param>
        private void HandleAdded(in Auditable auditable, in DateTimeOffset timestamp)
        {
            // We should do a safety-check on the existing values in case someone has manually changed the tracking state to 'Added',
            // or manually assigned the values (e.g. in unit tests).
            if (auditable.DateCreated == default || auditable.DateCreated == null)
                SetPrivateProperty(auditable, nameof(auditable.DateCreated), timestamp);
            if (CallerId is not null && (auditable.CreatedBy == default || auditable.CreatedBy == null || auditable.CreatedBy == Guid.Empty))
                SetPrivateProperty(auditable, nameof(auditable.CreatedBy), CallerId.Value);

            // There should not be any updated by info in new entities (as it's never been modified).
            SetPrivateProperty<Auditable, Guid?>(auditable, nameof(auditable.UpdatedBy), null);
            SetPrivateProperty<Auditable, DateTimeOffset?>(auditable, nameof(auditable.DateUpdated), null);
        }


        /// <summary>
        ///     Updates the details for entities with <see cref="Microsoft.EntityFrameworkCore.EntityState.Modified"/>.
        /// </summary>
        /// <param name="auditable"> The <see cref="Auditable"/> entity to be processed. </param>
        /// <param name="timestamp"> The creation timestamp. </param>
        private void HandleModified<T>(in T auditable, in DateTimeOffset timestamp) where T : Auditable
        {
            SetPrivateProperty(auditable, nameof(auditable.DateUpdated), timestamp);

            if (CallerId is not null)
                SetPrivateProperty(auditable, nameof(auditable.UpdatedBy), CallerId);
        }


        /// <summary>
        ///     Uses reflections to access and set a value in a private-property.
        /// </summary>
        /// <typeparam name="TEntity"> The class that is used in <paramref name="entity"/>. </typeparam>
        /// <typeparam name="TDatatype"> The datatype that is used in <paramref name="propertyName"/>. </typeparam>
        /// <param name="entity"> The entity we are using reflections on. </param>
        /// <param name="propertyName"> The '<c>nameof()</c>' for the private property that is accessed. </param>
        /// <param name="newValue"> The new value that will be set. </param>
        private void SetPrivateProperty<TEntity, TDatatype>(TEntity entity, string propertyName, TDatatype newValue)
        {
            PropertyInfo? propertyInfo = typeof(TEntity).GetProperty(propertyName);

            if (propertyInfo == null)
                return;

            propertyInfo.SetValue(entity, newValue);
        }
    }
}
