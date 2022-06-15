using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.ServiceModel
{
    public class ChangeAssetStatus
    {
        public IList<Guid> AssetLifecycleId { get; set; } = new List<Guid>();
        public Guid CallerId { get; set; }
    }
}
