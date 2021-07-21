using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategoryType
    {
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public bool IsChecked { get; set; }

        public IList<OrigoAssetCategoryLifecycleType> LifecycleTypes { get; set; }
    }
}
