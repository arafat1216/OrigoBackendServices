using ProductCatalog.Infrastructure.Infrastructure.Context;

namespace ProductCatalog.Infrastructure.Models.Database
{
    /// <summary>
    ///     Contains a human-readable list of the <see cref="Product.Id"/> values that's used when data-seeding 
    ///     a new <see cref="Product"/> in <see cref="ProductCatalogContext.SeedProductionData(Microsoft.EntityFrameworkCore.ModelBuilder)"/>.
    /// </summary>
    internal enum ProductSeedDataValues
    {
        SubscriptionManagement = 1,
        Implement = 2,
        FullLifecycle = 3,
        BookValue = 4,
        RecycleAndWipe = 5,
    }
}
