using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using System;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    public class NewHardwareServiceOrderDTO
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public OrderedByUserDTO OrderedBy { get; set; }
        public AssetInfo AssetInfo { get; set; }

        public NewHardwareServiceOrderDTO(NewHardwareServiceOrder order)
        {
            DeliveryAddress = order.DeliveryAddress;
            ErrorDescription = order.ErrorDescription;
            AssetInfo = order.AssetInfo;
        }
    }
}
