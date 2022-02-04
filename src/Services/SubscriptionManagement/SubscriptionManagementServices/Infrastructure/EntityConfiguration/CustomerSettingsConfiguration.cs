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
    internal class CustomerSettingsConfiguration : IEntityTypeConfiguration<CustomerSettings>
    {
        private bool _isSqlLite;
        public CustomerSettingsConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<CustomerSettings> builder)
        {
            //builder.ToTable("CustomerSettings");
            //builder.HasKey(x => x.Id);

            ////Properties

            ////Relationships
            //builder.HasMany(e => e.CustomerOperatorSettings);
        }
    }
}
