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
        /// A user provided description explaining the problem or reason for the service order.
        /// </summary>
        [Required]
        public string UserDescription { get; set; }

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
        /// Id(s) of the Service Addons or Extra Services that is/are supported by the Third party Service Provider. <para>
        /// 
        /// A <see cref="HardwareServiceOrderServices.Models.ServiceProvider"/> provides 1 or many <see cref="ServiceOrderAddon"/>. Some of them come as default. Some are selectable by Organization and some are selectable by the User.
        /// This list contains the <c>User selectable</c> service-addons.</para>
        /// </summary>
        public ISet<int> UserSelectedServiceOrderAddonIds { get; set; } = new HashSet<int>();
    }
}
