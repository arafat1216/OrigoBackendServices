
namespace ProductCatalog.Service.Models.Database.Interfaces
{
    internal interface IDbEntity
    {
        // TODO: Make this a shadow property
        Guid UpdatedBy { get; set; }
    }
}
