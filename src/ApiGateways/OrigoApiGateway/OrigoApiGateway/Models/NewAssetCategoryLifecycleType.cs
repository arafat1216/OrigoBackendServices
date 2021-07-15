using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class NewAssetCategoryLifecycleType
    {
        public Guid CustomerId { get; set; }
        public Guid AssetCategoryId { get; set; }
        public int LifecycleType { get; set; }
    }
}
