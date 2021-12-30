﻿using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class CustomerAssetCategoryDTO
    {
        public int AssetCategoryId { get; set; }

        public Guid CustomerId { get; set; }

        public IList<AssetCategoryLifecycleTypeDTO> LifecycleTypes { get; set; }
    }
}
