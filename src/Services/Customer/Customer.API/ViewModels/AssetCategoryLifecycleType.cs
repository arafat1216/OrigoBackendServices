using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public class AssetCategoryLifecycleType
    {
        public Guid CustomerId { get; set; }
        public Guid AssetCategoryId { get; set; }
        public int LifecycleType { get; set; }
    }
}
