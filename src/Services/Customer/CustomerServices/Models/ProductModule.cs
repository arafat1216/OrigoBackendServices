using Common.Seedwork;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        public string Name { get; set; }

        public ProductModuleGroup ProductModuleGroup { get; set; }
    }
}
