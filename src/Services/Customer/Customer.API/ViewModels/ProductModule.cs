using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class ProductModule
    {
        public string Name { get; set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; set; }
    }
}
