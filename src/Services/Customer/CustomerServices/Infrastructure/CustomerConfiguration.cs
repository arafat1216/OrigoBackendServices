using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure
{
    public class CustomerConfiguration :  IEntityTypeConfiguration<Organization>
    {
        public void Configure(EntityTypeBuilder<Organization> builder)
        {
            builder.ToTable("Customer");
            builder.Property(s => s.LastUpdatedDate).HasDefaultValueSql("CURRENT_TIMESTAMP");

        }
    }
}
