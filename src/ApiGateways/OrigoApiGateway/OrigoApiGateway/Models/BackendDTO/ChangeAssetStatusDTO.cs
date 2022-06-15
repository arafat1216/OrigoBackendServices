using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class ChangeAssetStatusDTO
    {
        public IList<Guid> AssetLifecycleId { get; set; } = new List<Guid>();
        public Guid CallerId { get; set; }
    }
}
