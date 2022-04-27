namespace Asset.API.ViewModels
{
    public record CategoryLifeCycleSetting
    {
        public int AssetCategoryId { get; init; }
        public string AssetCategoryName { get; init; }
        public decimal MinBuyoutPrice { get; init; }
    }
}
