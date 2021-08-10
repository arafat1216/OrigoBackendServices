using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public class UpdateProductModule
    {
        public Guid ProductModuleId { get; set; }

        public IList<Guid> ProductModuleGroupIds { get; set; }
    }
}
