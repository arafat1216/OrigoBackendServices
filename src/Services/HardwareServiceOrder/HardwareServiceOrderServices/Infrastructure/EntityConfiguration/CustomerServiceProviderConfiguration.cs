﻿using HardwareServiceOrderServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HardwareServiceOrderServices.Infrastructure.EntityConfiguration
{
    internal class CustomerServiceProviderConfiguration : EntityBaseConfiguration<CustomerServiceProvider>
    {

        public CustomerServiceProviderConfiguration(bool isSqlLite) : base(isSqlLite)
        {
        }


        /// <inheritdoc/>
        public override void Configure(EntityTypeBuilder<CustomerServiceProvider> builder)
        {
            // Call the parent that configures the shared properties from the inherited 'EntityV2' entity
            base.Configure(builder);

            /*
             * DB table configuration (keys, constraints, indexing, etc.)
             */

            builder.ToTable("CustomerServiceProvider");
            builder.HasComment("Configures a customer's service-provider settings.");

            builder.HasAlternateKey(x => new { x.CustomerId, x.ServiceProviderId });


            /*
             * Properties
             */

            // Add here
        }
    }
}
