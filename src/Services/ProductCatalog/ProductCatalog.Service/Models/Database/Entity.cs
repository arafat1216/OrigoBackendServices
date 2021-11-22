using ProductCatalog.Service.Models.Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProductCatalog.Service.Models.Database
{
    public abstract class Entity : IDbEntity
    {
        [Column(Order = 20)]
        public Guid UpdatedBy { get; set; }
    }
}
