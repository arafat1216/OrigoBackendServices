namespace Common.Enums
{
    /// <summary>
    ///     Contains a human-readable list of the <see cref="ProductType.Id"/> values that's used when data-seeding 
    ///     a new <see cref="ProductType"/> in <see cref="ProductCatalogContext.SeedProductionData(Microsoft.EntityFrameworkCore.ModelBuilder)"/>.
    /// </summary>
    public enum ProductTypeSeedDataValue
    {
        Unknown = 1,
        Module = 2,
        Option = 3
    }
}
