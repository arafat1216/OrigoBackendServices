using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    /// Information needed to create new Hardware Service Order
    /// </summary>
    public class NewHardwareServiceOrder
    {
        /// <summary>
        /// Represents the shipping address of the User who placed the Order. <see cref="DeliveryAddress"/>
        /// </summary>
        [Required]
        public DeliveryAddress DeliveryAddress { get; set; }
        
        /// <summary>
        /// Problem description of the device
        /// </summary>
        [Required]
        public string ErrorDescription { get; set; }
        
        /// <summary>
        /// Contact details of the User who placed the Order <see cref="ContactDetailsExtendedDTO"/>
        /// </summary>
        public ContactDetailsExtended OrderedBy { get; set; }
        
        /// <summary>
        /// Asset or Device related information. <see cref="AssetInfo"/>
        /// </summary>
        [Required]
        public AssetInfo AssetInfo { get; set; }
        
        /// <summary>
        /// Service Provider Id
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }
        
        /// <summary>
        /// Service Type Id. The value would one of values from ServiceTypeEnum
        /// </summary>
        [Required]
        public int ServiceTypeId { get; set; }
        
        /// <summary>
        /// Service Addons or Extra Services that is/are supported by the Service Provider. <see cref="ServiceOrderAddonsEnum"/>
        /// </summary>
        public List<ServiceOrderAddonsEnum> ServiceOrderAddons { get; set; } = new List<ServiceOrderAddonsEnum>();
    }
}
