﻿using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public class AssetCategoryLifecycleType : Entity
    {
        public Guid CustomerId { get; protected set; }
        public Guid AssetCategoryId { get; protected set; }
        public Guid LifecycleTypeId { get; protected set; }

        /// <summary>
        /// Create a connection between a customer and the lifecycle types enabled for their asset categories.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="assetCategoryId"></param>
        /// <param name="lifeCycleTypeId"></param>
        public AssetCategoryLifecycleType(Guid customerId, Guid assetCategoryId, Guid lifeCycleTypeId)
        {
            CustomerId = customerId;
            AssetCategoryId = assetCategoryId;
            LifecycleTypeId = lifeCycleTypeId;
        }

    }
}
