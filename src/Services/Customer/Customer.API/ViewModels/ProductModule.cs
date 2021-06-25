using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class ProductModule
    {
        public Guid ProductModuleId { get; set; }

        public string Name { get; set; }

        public IList<ProductModuleGroup> ProductModuleGroup { get; set; }
    }
}
