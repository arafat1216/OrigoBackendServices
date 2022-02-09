using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class CustomerOperatorSettingsConfiguration : IEntityTypeConfiguration<CustomerOperatorSettings>
    {
            private bool _isSqlLite;
            public CustomerOperatorSettingsConfiguration(bool isSqlLite)
            {
                _isSqlLite = isSqlLite;
            }

        public void Configure(EntityTypeBuilder<CustomerOperatorSettings> builder)
        {
            builder.ToTable("CustomerOperatorSettings");

            //Relationships
            builder.HasMany(e => e.AvailableSubscriptionProducts)
                .WithMany(e => e.CustomerOperatorSettings).UsingEntity(join => join.ToTable("CustomerOperatorSettingsJoin"));


        }
    }
}

