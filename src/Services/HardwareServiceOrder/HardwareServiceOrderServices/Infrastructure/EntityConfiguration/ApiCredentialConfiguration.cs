using HardwareServiceOrderServices.Models;
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


            builder.HasAlternateKey(x => new { x.CustomerServiceProviderId, x.ServiceTypeId });


            builder.HasOne(e => e.CustomerServiceProvider)
                   .WithMany(e => e.ApiCredentials)
                   .HasForeignKey(e => e.CustomerServiceProviderId);


            builder.HasOne(e => e.ServiceType)
                   .WithMany()
                   .HasForeignKey(e => e.ServiceTypeId);
        }
    }
}
