﻿using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class CustomerAssetCategoryDTO
    {
        public Guid AssetCategoryId { get; set; }

        public Guid OrganizationId { get; set; }

        public IList<AssetCategoryLifecycleTypeDTO> LifecycleTypes { get; set; }
    }
}
