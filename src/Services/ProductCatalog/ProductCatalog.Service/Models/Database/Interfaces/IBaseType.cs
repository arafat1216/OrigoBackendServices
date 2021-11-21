using ProductCatalog.Service.Models.Boilerplate;

namespace ProductCatalog.Service.Models.Database.Interfaces
{
    internal interface IBaseType
    {
        int Id { get; set; }
        //ICollection<ITranslation> Translations { get; set; }
    }
}
