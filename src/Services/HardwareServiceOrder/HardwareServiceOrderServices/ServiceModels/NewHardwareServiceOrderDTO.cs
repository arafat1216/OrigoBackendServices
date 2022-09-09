using HardwareServiceOrderServices.Models;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    /// This Dto contains necessary information which will be used to create a new Service Order
    /// </summary>
    public class NewHardwareServiceOrderDTO
    {
        /// <summary>
        /// Represents a single shipping address. <see cref="DeliveryAddressDTO"/>
        /// </summary>
        public DeliveryAddressDTO DeliveryAddress { get; set; }
        
        // TODO: This should be renamed
        /// <summary>
        /// Problem description of the device
        /// </summary>
        public string ErrorDescription { get; set; }

        // TODO: Should this be renamed?
        /// <summary>
        /// Contact details of the User who placed the Order <see cref="ContactDetailsExtendedDTO"/>
        /// </summary>
        public ContactDetailsExtendedDTO OrderedBy { get; set; }

        /// <summary>
        /// Asset related information. <see cref="AssetInfoDTO"/>
        /// </summary>
        public AssetInfoDTO AssetInfo { get; set; }
        
        
        /// <summary>
        /// Service Provider Identifier
        /// </summary>
        public int ServiceProviderId { get; set; }
        
        /// <summary>
        /// Service Type Id. The value would one of values from ServiceTypeEnum
        /// </summary>
        public int ServiceTypeId { get; set; }
        
        /// <summary>
        /// Service Addons or Extra Services that is/are supported by the Service Provider. <see cref="ServiceOrderAddonsEnum"/>
        /// </summary>
        public List<ServiceOrderAddonsEnum> ServiceOrderAddons { get; set; } = new List<ServiceOrderAddonsEnum>();
    }
}
