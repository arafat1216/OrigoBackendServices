using Common.Model;
using System;

namespace OrigoApiGateway.Models
{
    public record LifeCycleSetting
    {
        public Guid Id { get; init; }
        public Guid CustomerId { get; init; }
        public bool BuyoutAllowed { get; set; }
        public int AssetCategoryId { get; set; }
        public string AssetCategoryName { get; set; }
        public Money MinBuyoutPrice { get; set; }
        public string Currency { get; set; }
        public int Runtime { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
