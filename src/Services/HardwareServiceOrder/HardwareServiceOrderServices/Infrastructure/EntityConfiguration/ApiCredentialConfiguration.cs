using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class ApiCredentialConfiguration : EntityBaseConfiguration<ApiCredential>
    {

        public ApiCredentialConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<ApiCredential> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);

            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.ToTable(auditable => auditable.IsTemporal());

            builder.HasIndex(x => new { x.CustomerServiceProviderId, x.ServiceTypeId })
                   .IsUnique(true)
                   .HasFilter(null);


            /*
             * Properties
             */

            // We use it as an alternate key, so let's disallow changes to the property!
            builder.Property(e => e.CustomerServiceProviderId)
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

            // We use it as an alternate key, so let's disallow changes to the property!
            builder.Property(e => e.ServiceTypeId)
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);


            /*
             * Configure relationships
             */

            builder.HasOne(e => e.CustomerServiceProvider)
                   .WithMany(e => e.ApiCredentials)
                   .HasForeignKey(e => e.CustomerServiceProviderId);


            builder.HasOne(e => e.ServiceType)
                   .WithMany()
                   .HasForeignKey(e => e.ServiceTypeId)
                   .IsRequired(false);

        }
    }
}
