using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    public class NewHardwareServiceOrderDTO
    {
        public DeliveryAddressDTO DeliveryAddress { get; set; }
        
        // TODO: This should be renamed
        public string ErrorDescription { get; set; }

        // TODO: Should this be renamed?
        public ContactDetailsExtendedDTO OrderedBy { get; set; }

        public AssetInfoDTO AssetInfo { get; set; }
        
        public int ServiceProviderId { get; set; }
        public int ServiceTypeId { get; set; }
        public List<ServiceOrderAddonsEnum> ServiceOrderAddons { get; set; } = new List<ServiceOrderAddonsEnum>();
    }
}
