using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public record NewCustomerAssetCategoryTypeDTO
    {
        public int AssetCategoryId { get; set; }

        public Guid OrganizationId { get; set; }

        public IList<int> LifecycleTypes { get; set; }
        public Guid CallerId { get; set; }
    }
}
