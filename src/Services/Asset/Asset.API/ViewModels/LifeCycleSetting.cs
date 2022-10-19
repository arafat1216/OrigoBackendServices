using Common.Model;
using System;

namespace Asset.API.ViewModels
{
    public class LifeCycleSetting
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public string AssetCategoryName { get; init; }
        public int AssetCategoryId { get; init; }
        public bool BuyoutAllowed { get; set; }
        public Money MinBuyoutPrice { get; set; } = new();
        public int Runtime { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
