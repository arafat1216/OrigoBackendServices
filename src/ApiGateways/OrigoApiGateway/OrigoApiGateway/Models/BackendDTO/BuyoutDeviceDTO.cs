using System;

namespace OrigoApiGateway.Models.BackendDTO
{
    public class BuyoutDeviceDTO
    {
        public Guid AssetLifeCycleId { get; set; }
        public string PayrollContactEmail { get; set; } = string.Empty;
        public Guid CallerId { get; set; }
    }
}
