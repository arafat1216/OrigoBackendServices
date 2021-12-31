using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    /// <summary>
    /// Response object
    /// </summary>
    public class OrigoCustomerAssetCategoryType
    {
        public string Name { get; set; }

        public int AssetCategoryId { get; set; }

        public bool IsChecked { get; set; }

        public IList<OrigoAssetCategoryLifecycleType> LifecycleTypes { get; set; }
    }
}
