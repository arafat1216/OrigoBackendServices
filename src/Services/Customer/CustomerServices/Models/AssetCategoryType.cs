﻿using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public class AssetCategoryType : Entity
    {
        protected AssetCategoryType() { }

        public AssetCategoryType(Guid assetCategoryId, Guid customerId, IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            AssetCategoryId = assetCategoryId;
            ExternalCustomerId = customerId;
            LifecycleTypes = lifecycleTypes;
        }

        public Guid AssetCategoryId { get; protected set; }

        public Guid ExternalCustomerId { get; protected set; }

        public IList<AssetCategoryLifecycleType> LifecycleTypes { get; protected set; }

        public void UpdateCustomerId(Guid customerId)
        {
            ExternalCustomerId = customerId;
        }

        public void SetAssetCategoryId(Guid assetCategoryId)
        {
            AssetCategoryId = assetCategoryId;
        }

        public void SetLifecycleTypes(IList<AssetCategoryLifecycleType> lifecycleTypes)
        {
            LifecycleTypes = lifecycleTypes;
        }
    }
}
