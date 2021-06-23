using Common.Seedwork;
using System.Collections.Generic;

namespace CustomerServices.Models
{
    public class ProductModule : Entity, IAggregateRoot
    {
        public string Name { get; set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; set; }
    }
}
