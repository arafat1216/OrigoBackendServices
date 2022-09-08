using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrder.API.ViewModels
{
    public class NewHardwareServiceOrder
    {
        public DeliveryAddress DeliveryAddress { get; set; }
        public string ErrorDescription { get; set; }
        public ContactDetailsExtended OrderedBy { get; set; }
        public AssetInfo AssetInfo { get; set; }
        public int ServiceProviderId { get; set; }
        public int ServiceTypeId { get; set; }
        
        public List<ServiceOrderAddonsEnum> ServiceOrderAddons { get; set; } = new List<ServiceOrderAddonsEnum>();
    }
}
