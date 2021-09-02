using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class UpdateProductModule
    {
        public Guid ProductModuleId { get; set; }

        public IList<Guid> ProductModuleGroupIds { get; set; }
    }
}
