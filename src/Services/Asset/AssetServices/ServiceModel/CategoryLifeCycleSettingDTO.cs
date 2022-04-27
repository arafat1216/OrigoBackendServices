namespace AssetServices.ServiceModel
{
    public record CategoryLifeCycleSettingDTO
    {
        public int AssetCategoryId { get; set; }
        public string AssetCategoryName { get; set; }
        public decimal MinBuyoutPrice { get; set; } = decimal.Zero;
    }
}
