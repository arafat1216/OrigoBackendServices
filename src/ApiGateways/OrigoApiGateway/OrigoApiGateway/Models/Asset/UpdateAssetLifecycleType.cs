using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.Asset
{
    /// <summary>
    /// Information required to update an asset's lifecycletype
    /// </summary>
    public class UpdateAssetLifecycleType
    {
        public Guid AssetId { get; set; }
        public Guid CallerId { get; set; }
        public int LifecycleType { get; set; }
    }
}
