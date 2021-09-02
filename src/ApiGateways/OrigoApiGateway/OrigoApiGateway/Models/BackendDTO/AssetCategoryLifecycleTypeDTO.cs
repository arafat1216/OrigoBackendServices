using Common.Enums;
using System;

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
