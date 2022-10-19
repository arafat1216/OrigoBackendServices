using AssetServices.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AssetServices.Infrastructure.EntityConfiguration;

internal class SalaryDeductionTransactionConfiguration : EntityBaseConfiguration<SalaryDeductionTransaction>
{
    public SalaryDeductionTransactionConfiguration(bool isSqLite) : base(isSqLite)
    {
    }


    /// <inheritdoc/>
    public override void Configure(EntityTypeBuilder<SalaryDeductionTransaction> builder)
    {
        // Call the parent that configures the shared properties from the inherited 'Entity'-class
        base.Configure(builder);


        /*
         * DB table configuration (keys, constraints, indexing, etc.)
         */

        // Add as needed.


        /*
         * Configure properties
         */

        builder.OwnsOne(a => a.Deduction, b =>
        {
            b.Property(p => p.CurrencyCode).HasConversion(CurrencyConverter);
        });
    }
}