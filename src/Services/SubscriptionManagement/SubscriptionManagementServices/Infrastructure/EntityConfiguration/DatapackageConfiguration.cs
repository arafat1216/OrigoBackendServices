using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SubscriptionManagementServices.Models;

namespace SubscriptionManagementServices.Infrastructure.EntityConfiguration
{
    internal class DataPackageConfiguration : IEntityTypeConfiguration<DataPackage>
    {
        private readonly bool _isSqlLite;
        public DataPackageConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<DataPackage> builder)
        {
            builder.ToTable("DataPackage");

            //Properties
            builder.Property(x => x.DataPackageName).IsRequired().HasMaxLength(50);
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
            builder.Property(s => s.CreatedDate).HasDefaultValueSql(_isSqlLite ? "CURRENT_TIMESTAMP" : "SYSUTCDATETIME()");
        }
    }
}
