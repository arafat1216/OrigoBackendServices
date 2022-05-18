namespace OrigoApiGateway.Models.Asset
{
    public class MinBuyoutPrice
    {
        public string Country { get; init; }
        public int AssetCategoryId { get; init; }
        public decimal Amount { get; init; }
        public string Currency { get; set; }
    }
}
