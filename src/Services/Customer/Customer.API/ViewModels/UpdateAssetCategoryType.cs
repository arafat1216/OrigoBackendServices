﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public class UpdateAssetCategoryType
    {
        public Guid AssetCategoryId { get; set; }

        public Guid CustomerId { get; set; }

        public IList<int> LifecycleTypes { get; set; }
    }
}
