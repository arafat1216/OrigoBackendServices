using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration
{
    /// <summary>
    ///     EntityFramwork configurations for the <see cref="Department"/> entity.
    /// </summary>
    internal class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        private readonly bool _isSqlLite;

        public DepartmentConfiguration(bool isSqlLite)
        {
            _isSqlLite = isSqlLite;
        }

        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Department");

            /*
             * Properties
             */

            builder.Property(e => e.ExternalDepartmentId)
                   .HasDefaultValueSql("NEWID()")
                   .ValueGeneratedOnAdd()
                   .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Throw);
        }

    }
}
