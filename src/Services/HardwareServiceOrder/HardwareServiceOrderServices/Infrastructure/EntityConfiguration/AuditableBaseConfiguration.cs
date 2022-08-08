using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    /// <summary>
    ///     Implements the <see cref="IEntityTypeConfiguration{TEntity}"/> for the given <typeparamref name="T"/> entity-type,
    ///     and configures all shared properties from <see cref="Auditable"/>. <para>
    ///     
    ///     To configure the inherited entity, overwrite the <see cref="Configure(EntityTypeBuilder{T})"/> method, and add a call
    ///     to "<c>base.Configure(builder)</c>" inside the override. </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class AuditableBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : Auditable
    {
        /// <summary>
        ///     Set during creation, and indicates if the <see cref="HardwareServiceOrderContext"/> is using
        ///     SQLite (unit-testing) or SQL Server.
        /// </summary>
        protected readonly bool _isSqlLite;

        protected AuditableBaseConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }


        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            /*
             * Properties
             */

            if (_isSqlLite)
            {
                builder.Property(auditable => auditable.DateCreated)
                       .HasConversion(new DateTimeOffsetToBinaryConverter())
                       .HasDefaultValueSql("CURRENT_TIMESTAMP")
                       .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

                builder.Property(auditable => auditable.DateUpdated)
                       .HasConversion(new DateTimeOffsetToBinaryConverter());

                builder.Property(auditable => auditable.DateDeleted)
                       .HasConversion(new DateTimeOffsetToBinaryConverter());
            }
            else
            {
                builder.Property(auditable => auditable.DateCreated)
                       .HasDefaultValueSql("SYSUTCDATETIME()")
                       .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            }

            builder.Property(auditable => auditable.CreatedBy)
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);


            /*
             * Global query filter
             */

            builder.HasQueryFilter(auditable => !auditable.IsDeleted);
        }
    }
}
