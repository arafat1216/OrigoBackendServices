using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class DatapackageConfiguration : IEntityTypeConfiguration<Datapackage>
    {
        public void Configure(EntityTypeBuilder<Datapackage> builder)
        {
            builder.ToTable("Datapackage");

            //Properties
            builder.Property(x => x.DatapackageName).HasMaxLength(50);
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("SYSUTCDATETIME()");
        }
    }
}
