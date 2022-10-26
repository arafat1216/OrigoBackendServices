using Common.Model;
using System;

namespace AssetServices.ServiceModel
{
    public record LifeCycleSettingDTO
    {
        public Guid ExternalId { get; init; }
        public Guid CustomerId { get; init; }
        public string AssetCategoryName { get; init; }
        public int AssetCategoryId { get; init; }
        public bool BuyoutAllowed { get; set; }
        public Money MinBuyoutPrice { get; set; } = new();
        public DateTime CreatedDate { get; set; }
        public int? Runtime { get; set; } = 36;
    }
}
