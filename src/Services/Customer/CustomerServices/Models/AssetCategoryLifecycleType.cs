using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public class AssetCategoryLifecycleType : Entity
    {
        /// <summary>
        /// Create a connection between a customer and the lifecycle types enabled for their asset categories.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="assetCategoryId"></param>
        /// <param name="lifeCycleTypeId"></param>
        public AssetCategoryLifecycleType(Guid assetCategoryId, string name)
        {
            AssetCategoryLifecycleId = assetCategoryId;
            Name = name;
        }

        protected AssetCategoryLifecycleType() { }
        
        public Guid AssetCategoryLifecycleId { get; protected set; }

        public string Name { get; protected set; }

        public ICollection<Customer> Customers { get; set; }
    }
}
