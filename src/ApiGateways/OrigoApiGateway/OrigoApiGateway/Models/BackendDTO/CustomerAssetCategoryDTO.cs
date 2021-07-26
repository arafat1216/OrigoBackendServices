using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class CustomerAssetCategoryDTO
    {
        public Guid AssetCategoryId { get; set; }

        public Guid CustomerId { get; set; }

        public IList<AssetCategoryLifecycleTypeDTO> LifecycleTypes { get; set; }
    }
}
