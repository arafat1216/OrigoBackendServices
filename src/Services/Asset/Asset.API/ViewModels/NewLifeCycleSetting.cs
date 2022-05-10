using System;

namespace Asset.API.ViewModels
{
    public class NewLifeCycleSetting
    {
        public int AssetCategoryId { get; set; }
        public bool BuyoutAllowed { get; set; }
        public decimal MinBuyoutPrice { get; set; }
        public Guid CallerId { get; set; }
    }
}
