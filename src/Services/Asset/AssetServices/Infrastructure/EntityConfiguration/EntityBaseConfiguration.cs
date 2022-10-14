using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration
{
    /// <summary>
    ///     Implements the <see cref="IEntityTypeConfiguration{TEntity}"/> for the given <typeparamref name="T"/> entity-type,
    ///     and configures all shared properties from <see cref="Entity"/>-class. <para>
    ///     
    ///     To configure the inherited entity, overwrite the <see cref="Configure(EntityTypeBuilder{T})"/> method, and add a call
    ///     to "<c>base.Configure(builder)</c>" inside the override. </para>
    /// </summary>
    /// <typeparam name="T"> The entity-type to be configured. </typeparam>
    internal abstract class EntityBaseConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
    {
        /// <summary>
        ///     Set during creation, and indicates if the <see cref="AssetsContext"/> is using
        ///     SQLite (unit-testing) or SQL Server.
        /// </summary>
        protected readonly bool _isSqLite;

        protected EntityBaseConfiguration(bool isSqLite)
        {
            _isSqLite = isSqLite;
        }


        /// <inheritdoc/>
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            // TODO: Enable this when the global query filter below is added.
            //builder.HasIndex(entity => entity.IsDeleted);


            /*
             * Configure properties
             */

            builder.Property(s => s.LastUpdatedDate)
                   .HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");

            builder.Property(s => s.CreatedDate)
                   .HasDefaultValueSql(_isSqLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");


            /*
             * Configure global queries
             */

            // TODO: Enable this once we have verified that it don't break any existing functionality.
            // builder.HasQueryFilter(entity => entity.IsDeleted == false);
        }
    }
}
