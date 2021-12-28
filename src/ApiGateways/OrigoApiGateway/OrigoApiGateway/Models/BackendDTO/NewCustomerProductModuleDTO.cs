using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewCustomerProductModuleDTO
    {
        public Guid ProductModuleId { get; set; }

        public IList<Guid> ProductModuleGroupIds { get; set; }

        public Guid CallerId { get; set; }

    }
}
