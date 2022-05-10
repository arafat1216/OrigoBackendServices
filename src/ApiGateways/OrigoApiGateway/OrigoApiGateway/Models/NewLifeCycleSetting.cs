using System.Collections.Generic;

namespace OrigoApiGateway.Models
{
    public class NewLifeCycleSetting
    {
        public bool BuyoutAllowed { get; set; }
        public int AssetCategoryId { get; set; }
        public string AssetCategoryName { get; set; }
        public decimal MinBuyoutPrice { get; set; }
    }
}
