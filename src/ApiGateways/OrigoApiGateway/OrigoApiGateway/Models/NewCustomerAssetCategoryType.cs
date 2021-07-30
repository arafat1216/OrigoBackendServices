using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class NewCustomerAssetCategoryType
    {
        public Guid AssetCategoryId { get; set; }

        public Guid CustomerId { get; set; }

        public IList<int> LifecycleTypes { get; set; }
    }
}
