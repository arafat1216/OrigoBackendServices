using Common.Seedwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Models
{
    public class AssetCategoryType : Entity
    {
        public Guid AssetCategoryId { get; set; }

        public Guid ExternalCustomerId { get; set; }

        public IList<AssetCategoryLifecycleType> LifecycleTypes { get; set; }
    }
}
