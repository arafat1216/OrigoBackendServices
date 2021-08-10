using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class NewCustomerProductModule
    {
        public Guid ProductModuleId { get; set; }

        public IList<Guid> ProductModuleGroupIds { get; set; }
    }
}
