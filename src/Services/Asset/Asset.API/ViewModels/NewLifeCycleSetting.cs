using Common.Model;
using System;

namespace Asset.API.ViewModels
{
    public class NewLifeCycleSetting
    {
        public int AssetCategoryId { get; set; }
        public bool BuyoutAllowed { get; set; }
        public Money MinBuyoutPrice { get; set; } = new();
        public int? Runtime { get; set; } = 36;
        public Guid CallerId { get; set; }
    }
}
