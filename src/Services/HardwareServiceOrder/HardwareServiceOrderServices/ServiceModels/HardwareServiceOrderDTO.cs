using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    public class HardwareServiceOrderDTO
    {
        public DeliveryAddressDTO DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public ContactDetailsExtendedDTO OrderedBy { get; set; }
        public AssetInfoDTO AssetInfo { get; set; }
    }
}
