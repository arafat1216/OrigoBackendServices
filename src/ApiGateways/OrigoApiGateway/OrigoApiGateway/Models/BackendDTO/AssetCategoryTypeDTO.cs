using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryTypeDTO
    {
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public IList<AssetCategoryLifecycleTypeDTO> LifecycleTypes { get; set; }
    }
}
