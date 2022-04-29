namespace OrigoApiGateway.Models
{
    public class NewCategoryLifeCycleSetting
    {
        public int AssetCategoryId { get; set; }
        public decimal MinBuyoutPrice { get; set; } = decimal.Zero;
    }
}
