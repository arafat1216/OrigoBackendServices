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
        [Obsolete("No longer a feature")]
        BasicSubscriptionManagement = 3,
        [Obsolete("No longer a feature")]
        BasicNonPersonalAssetManagement = 4,
        [Obsolete("No longer a feature")]
        BasicBookValueManagement = 5,
        [Obsolete("No longer a feature")]
        BasicTransactionalAssetReturn = 6,
        RecycleAndWipeAssetReturn = 7,
        BasicHardwareRepair = 8,
        EmployeeAccess = 9,
        DepartmentStructure = 10,
        OnAndOffboarding = 11,
        BuyoutAsset = 12,
        AssetManagement = 13,
        SubscriptionManagement = 14,
        AssetBookValue = 15,
        InternalAssetReturn = 16,
        WebshopAccess = 17
    }
}
