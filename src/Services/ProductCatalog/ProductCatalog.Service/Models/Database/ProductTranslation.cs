using ProductCatalog.Service.Models.Boilerplate;

namespace ProductCatalog.Service.Models.Database
{
    internal class ProductTranslation : Translation
    {
        // EF DB Columns
        public int ProductId { get; set; }
    }
}
