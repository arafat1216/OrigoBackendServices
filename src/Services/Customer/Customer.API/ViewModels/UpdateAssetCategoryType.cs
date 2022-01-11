using System;
using System.Collections.Generic;

namespace Customer.API.ViewModels
{
    public class UpdateAssetCategoryType
    {
        public int AssetCategoryId { get; set; }

        public Guid OrganizationId { get; set; }

        public IList<int> LifecycleTypes { get; set; }
        public Guid CallerId { get; set; }
    }
}
