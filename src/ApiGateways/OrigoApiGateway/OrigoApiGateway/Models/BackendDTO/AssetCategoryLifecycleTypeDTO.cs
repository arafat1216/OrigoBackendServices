using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryLifecycleTypeDTO
    {
        public string Name { get; set; }

        public Guid CustomerId { get; set; }

        public Guid AssetCategoryId { get; set; }

        public LifecycleType LifecycleType { get; set; }
    }
}
