using System;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    public class OrigoHardwareServiceOrder
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public Guid AssetLifecycleId { get; set; }
        public Guid OwnerId { get; set; }
        public string? ExternalServiceManagementLink { get; set; }
    }
}
