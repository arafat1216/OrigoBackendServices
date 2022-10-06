using CustomerServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustomerServices.Infrastructure.Context.EntityConfiguration;



internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    private readonly bool _isSqlLite;

    public UserConfiguration(bool isSqlLite)
    {
        _isSqlLite = isSqlLite;
    }

    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");

        builder.HasIndex(e => e.Email).IsUnique();

        /*
         * Properties
         */

        builder.Property(e => e.UserId).HasDefaultValueSql("NEWID()").ValueGeneratedOnAdd().Metadata
            .SetAfterSaveBehavior(PropertySaveBehavior.Throw);

        /*
         * Relationships / Navigation
         */

        builder.HasMany(u => u.ManagesDepartments).WithMany(d => d.Managers)
            .UsingEntity(join => join.ToTable("DepartmentManager"));

        builder.HasOne(e => e.Department).WithMany(d => d.Users);
    }
}