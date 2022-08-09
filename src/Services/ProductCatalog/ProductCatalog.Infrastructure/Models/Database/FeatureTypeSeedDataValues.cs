using ProductCatalog.Infrastructure.Infrastructure.Context;

namespace ProductCatalog.Infrastructure.Models.Database
{
    /// <summary>
    ///     Contains a human-readable list of the <see cref="FeatureType.Id"/> values that's used when data-seeding 
    ///     a new <see cref="FeatureType"/> in <see cref="ProductCatalogContext.SeedProductionData(Microsoft.EntityFrameworkCore.ModelBuilder)"/>.
    /// </summary>
    internal enum FeatureTypeSeedDataValues
    {
        Unknown = 1,
        AssetReturn = 2,
    }
}
