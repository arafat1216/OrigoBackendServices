using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class AssetCategoryType
    {
        public Guid AssetCategoryId { get; set; }

        public Guid CustomerId { get; set; }

        public IList<AssetCategoryLifecycleType> LifecycleTypes { get; set; }
    }
}
