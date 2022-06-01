using System;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request
{
    public class NewHardwareServiceOrder
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public AssetInfo AssetInfo { get; set; }
    }
}
