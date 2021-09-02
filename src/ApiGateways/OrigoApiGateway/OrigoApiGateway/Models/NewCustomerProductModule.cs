using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class NewCustomerProductModule
    {
        public Guid ProductModuleId { get; set; }

        public IList<Guid> ProductModuleGroupIds { get; set; }
    }
}
