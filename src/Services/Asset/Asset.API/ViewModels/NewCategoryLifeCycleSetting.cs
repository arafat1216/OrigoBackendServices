using System;

namespace Asset.API.ViewModels
{
    public class NewCategoryLifeCycleSetting
    {
        public int AssetCategoryId { get; set; }
        public decimal MinBuyoutPrice { get; set; } = decimal.Zero;
        public Guid CallerId { get; set; }

    }
}
