using Common.Seedwork;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace HardwareServiceOrderServices.Infrastructure
{
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
        public SaveContextChangesInterceptor(Guid callerId) : base()
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
            var newEntities = eventData.Context.ChangeTracker.Entries()
                                                             .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Added)
                                                             .Select(e => e.Entity);

            var modifiedEntities = eventData.Context.ChangeTracker.Entries()
                                                                  .Where(e => e.State == Microsoft.EntityFrameworkCore.EntityState.Modified)
                                                                  .Select(e => e.Entity);


            foreach (var entity in newEntities)
            {
                var auditableEntity = entity as Auditable;

                if (auditableEntity is not null)
                {
                    // There should not be any updated by info in new entities (as it's never been modified).
                    auditableEntity.UpdatedBy = null;
                    auditableEntity.DateUpdated = null;


                    auditableEntity.DateCreated = DateTimeOffset.UtcNow;

                    if (CallerId is not null && (auditableEntity.CreatedBy == null || auditableEntity.CreatedBy == Guid.Empty))
                        auditableEntity.CreatedBy = CallerId.Value;
                }
            }


            foreach (var entity in modifiedEntities)
            {
                var auditableEntity = entity as Auditable;

                if (auditableEntity is not null)
                {
                    if (CallerId is not null)
                        auditableEntity.UpdatedBy = CallerId;

                    auditableEntity.DateUpdated = DateTimeOffset.UtcNow;
                }
            }
        }
    }
}
