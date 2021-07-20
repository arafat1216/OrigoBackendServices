using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Customer.API.ViewModels
{
    public class NewAssetCategoryLifecycleType
    {
        public Guid CustomerId { get; set; }
        public Guid AssetCategoryId { get; set; }
        public string LifecycleType { get; set; }
    }
}
