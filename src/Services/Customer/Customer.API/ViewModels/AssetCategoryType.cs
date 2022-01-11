using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class AssetCategoryType
    {
        public int AssetCategoryId { get; set; }

        public Guid OrganizationId { get; set; }

        public IList<AssetCategoryLifecycleType> LifecycleTypes { get; set; }
    }
}
