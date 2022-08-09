using ProductCatalog.Infrastructure.Infrastructure.Context;

namespace ProductCatalog.Infrastructure.Models.Database
{
    /// <summary>
    ///     Contains a human-readable list of the <see cref="Feature.Id"/> values that's used when data-seeding 
    ///     a new <see cref="Feature"/> in <see cref="ProductCatalogContext.SeedProductionData(Microsoft.EntityFrameworkCore.ModelBuilder)"/>.
    /// </summary>
    internal enum FeatureSeedDataValues
    {
        BasicUserManagement = 1,
        BasicAssetManagement = 2,
        BasicSubscriptionManagement = 3,
        BasicNonPersonalAssetManagement = 4,
        BasicBookValueManagement = 5,
        BasicTransactionalAssetReturn = 6,
        RecycleAndWipeAssetReturn = 7,
        BasicHardwareRepair = 8,
    }
}
