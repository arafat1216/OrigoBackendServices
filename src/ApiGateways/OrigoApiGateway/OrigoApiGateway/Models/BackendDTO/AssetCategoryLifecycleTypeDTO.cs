using Common.Enums;
using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class AssetCategoryLifecycleTypeDTO
    {
        public string Name { get; init; }

        public Guid OrganizationId { get; init; }

        public int AssetCategoryId { get; init; }

        public LifecycleType LifecycleType { get; init; }
    }
}
