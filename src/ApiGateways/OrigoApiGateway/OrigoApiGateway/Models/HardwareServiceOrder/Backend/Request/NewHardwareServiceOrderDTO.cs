using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using System;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    public class NewHardwareServiceOrderDTO
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public ContactDetailsExtended OrderedBy { get; set; }
        public AssetInfo AssetInfo { get; set; }
        
        public int ServiceProviderId { get; set; }

        public int ServiceTypeId { get; set; }
        
        public List<int> ServiceOrderAddons { get; set; } = new List<int>();

        public NewHardwareServiceOrderDTO(NewHardwareServiceOrder order)
        {
            DeliveryAddress = order.DeliveryAddress;
            ErrorDescription = order.ErrorDescription;
        }
    }
}
