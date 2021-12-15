using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.Asset
{
    public class AssignAssetToUser
    {
        public Guid AssetId { get; set; }
        public Guid? UserId { get; set; }
        public Guid CallerId { get; set; }
    }
}
