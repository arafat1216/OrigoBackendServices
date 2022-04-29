using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class NewCategoryLifeCycleSettingDTO
    {
        public int AssetCategoryId { get; set; }
        public decimal MinBuyoutPrice { get; set; }
        public Guid CallerId { get; set; }

    }
}
