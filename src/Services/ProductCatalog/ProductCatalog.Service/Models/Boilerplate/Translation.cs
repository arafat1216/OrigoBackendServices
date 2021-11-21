using ProductCatalog.Service.Models.Database.Interfaces;

namespace ProductCatalog.Service.Models.Boilerplate
{
    public class Translation : ITranslation
    {
        public string Language { get; set; }
        public string Name { get; set; }
        public string? Description { get ; set ; }
    }
}
