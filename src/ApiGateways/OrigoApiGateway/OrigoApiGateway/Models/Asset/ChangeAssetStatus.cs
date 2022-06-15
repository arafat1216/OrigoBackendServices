using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.Asset
{
    public class ChangeAssetStatus
    {
        public IList<Guid> AssetLifecycleId { get; set; } = new List<Guid>();
    }
}
