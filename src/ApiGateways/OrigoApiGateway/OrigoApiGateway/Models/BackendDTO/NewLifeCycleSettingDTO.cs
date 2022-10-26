using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class NewLifeCycleSettingDTO
    {
        public bool BuyoutAllowed { get; set; }
        public int AssetCategoryId { get; set; }
        public decimal MinBuyoutPrice { get; set; }
        public int? Runtime { get; set; } = 36;
        public Guid CallerId { get; set; }
    }
}
