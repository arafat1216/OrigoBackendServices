using Common.Enums;
using System;

namespace Customer.API.ViewModels
{
    public class AssetCategoryLifecycleType
    {
        public string Name { get; set; }
        public Guid CustomerId { get; set; }
        public Guid AssetCategoryId { get; set; }
        public LifecycleType LifecycleType { get; set; }
    }
}
