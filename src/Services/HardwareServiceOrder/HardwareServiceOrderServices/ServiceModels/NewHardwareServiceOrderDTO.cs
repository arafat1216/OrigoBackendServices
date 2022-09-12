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
        /// Id(s) of the Service Addons or Extra Services that is/are supported by the Third party Service Provider. <para>
        /// 
        /// A <see cref="HardwareServiceOrderServices.Models.ServiceProvider"/> provides 1 or many <see cref="ServiceOrderAddon"/>. Some of them come as default. Some are selectable by Organization and some are selectable by the User.
        /// This list contains the <c>User selectable</c> service-addons.</para>
        /// </summary>
        public ISet<int> UserSelectedServiceOrderAddonIds { get; set; } = new HashSet<int>();
    }
}
