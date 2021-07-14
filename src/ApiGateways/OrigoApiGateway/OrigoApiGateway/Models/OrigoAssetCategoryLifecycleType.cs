using OrigoApiGateway.Models.BackendDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrigoApiGateway.Models
{
    public class OrigoAssetCategoryLifecycleType
    {
        public Guid CustomerId { get; protected set; }
        public Guid AssetCategoryId { get; protected set; }
        public int LifecycleType { get; protected set; }

        public OrigoAssetCategoryLifecycleType(AssetCategoryLifecycleTypeDTO assetCategoryLifecycleType)
        {
            CustomerId = assetCategoryLifecycleType.CustomerId;
            AssetCategoryId = assetCategoryLifecycleType.AssetCategoryId;
            LifecycleType = assetCategoryLifecycleType.LifecycleType;
        }
    }
}
