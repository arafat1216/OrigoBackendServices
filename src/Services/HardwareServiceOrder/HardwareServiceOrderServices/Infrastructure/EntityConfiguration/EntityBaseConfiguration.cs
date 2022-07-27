using Common.Seedwork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    /// <summary>
    ///     Implements the <see cref="IEntityTypeConfiguration{TEntity}"/> for the given <typeparamref name="T"/> entity-type,
    ///     and configures all shared properties from <see cref="EntityV2"/> and <see cref="Auditable"/>. <para>
    ///     
    ///     To configure the inherited entity, overwrite the <see cref="Configure(EntityTypeBuilder{T})"/> method, and add a call
    ///     to "<c>base.Configure(builder)</c>" inside the override. </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal abstract class EntityBaseConfiguration<T> : AuditableBaseConfiguration<T> where T : EntityV2
    {
        protected EntityBaseConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<T> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'Auditable' entity
            base.Configure(builder);


            /*
             * Properties
             */

            // Configure all ID columns so the auto-incremental value starts on 1000.
            // This frees up a wide range of values that can be used for seeding-data.
            builder.Property(e => e.Id)
                   .ValueGeneratedOnAdd()
                   .UseIdentityColumn(1000);
        }
    }
}
