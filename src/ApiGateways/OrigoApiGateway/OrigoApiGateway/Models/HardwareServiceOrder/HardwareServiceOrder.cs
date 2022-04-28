using System;

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class HardwareServiceOrder
    {
        public Guid Id { get; set; }

        public Guid AssetLifecycleId { get; set; }

        public DeliveryAddress? DeliveryAddress { get; set; }
        public string UserDescription { get; set; }

        public string? ExternalProviderLink { get; set; }

        public string ServiceType { get; set; }

        public string Status { get; set; }

        public string ServiceProvider{ get; init; }
    }
}
