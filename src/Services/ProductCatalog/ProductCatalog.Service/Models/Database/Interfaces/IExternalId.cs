
namespace ProductCatalog.Service.Models.Database.Interfaces
{
    internal interface IExternalId : IDbEntity
    {
        public Guid ExternalId { get; set; }
    }
}
