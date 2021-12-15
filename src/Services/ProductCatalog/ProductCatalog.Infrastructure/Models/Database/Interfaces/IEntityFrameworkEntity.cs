namespace ProductCatalog.Infrastructure.Models.Database.Interfaces
{
    /// <summary>
    ///     Represents a entity that is recognized by a Entity Framework DbContext, and therefore can be used inside the generic repository.
    /// </summary>
    /// <remarks>
    ///     The interface is also used to add any properties (and enforce the consistency of these) that should exist in all database-models.
    /// </remarks>
    internal interface IEntityFrameworkEntity
    {
        Guid UpdatedBy { get; set; }
    }
}
