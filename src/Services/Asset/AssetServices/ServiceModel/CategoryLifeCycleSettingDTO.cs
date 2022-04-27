namespace AssetServices.ServiceModel
{
    public record CategoryLifeCycleSettingDTO
    {
        public int AssetCategoryId { get; set; }
        public int AssetCategoryName { get; set; }
        public decimal MinBuyoutPrice { get; set; } = decimal.Zero;
    }
}
