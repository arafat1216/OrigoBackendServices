using Common.Enums;
using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryLifecycleTypeDTO
    {
        public string Name { get; set; }

        public Guid OrganizationId { get; set; }

        public int AssetCategoryId { get; set; }

        public LifecycleType LifecycleType { get; set; }
    }
}
