﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public class AssetCategoryType
    {
        public Guid AssetCategoryId { get; set; }

        public string Name { get; set; }

        public IList<AssetCategoryLifecycleType> LifecycleTypes { get; set; }
    }
}
