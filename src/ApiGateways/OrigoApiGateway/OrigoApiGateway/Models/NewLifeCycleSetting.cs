using Common.Model;

namespace OrigoApiGateway.Models
{
    public class NewLifeCycleSetting
    {
        public bool BuyoutAllowed { get; set; }
        public int AssetCategoryId { get; set; }
        public Money MinBuyoutPrice { get; set; }
        public int? Runtime { get; set; } = 36;
    }
}
