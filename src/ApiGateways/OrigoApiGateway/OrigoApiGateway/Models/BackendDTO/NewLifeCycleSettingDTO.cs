using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class NewLifeCycleSettingDTO
    {
        public bool BuyoutAllowed { get; set; }
        public Guid CallerId { get; set; }
    }
}
