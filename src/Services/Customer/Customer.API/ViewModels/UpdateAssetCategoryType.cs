using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class UpdateAssetCategoryType
    {
        public Guid AssetCategoryId { get; set; }

        public Guid CustomerId { get; set; }

        public IList<int> LifecycleTypes { get; set; }
        public Guid CallerId { get; set; }
    }
}
