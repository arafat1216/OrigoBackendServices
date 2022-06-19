using System;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request
{
    public class NewHardwareServiceOrder
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public Guid AssetId { get; set; }
    }
}
