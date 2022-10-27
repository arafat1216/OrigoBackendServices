namespace Common.Enums
{
    /// <summary>
    ///     Contains a human-readable list of the <see cref="Product.Id"/> values that's used when data-seeding 
    ///     a new <see cref="Product"/> in <see cref="ProductCatalogContext.SeedProductionData(Microsoft.EntityFrameworkCore.ModelBuilder)"/>.
    /// </summary>
    public enum ProductSeedDataValues
    {
        SubscriptionManagement = 1,
        Implement = 2,
        TransactionalDeviceLifecycleManagement = 3,
        BookValue = 4,
        RecycleAndWipe = 5,
    }
}
