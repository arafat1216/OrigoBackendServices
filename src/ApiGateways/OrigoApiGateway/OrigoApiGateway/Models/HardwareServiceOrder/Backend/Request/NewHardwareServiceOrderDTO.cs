using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    /// <summary>
    /// Creates new Hardware Service Order.
    /// </summary>
    public class NewHardwareServiceOrderDTO
    {
        /// <summary>
        /// Delivery address of the order
        /// </summary>
        [Required]
        public DeliveryAddress DeliveryAddress { get; set; }

        /// <summary>
        /// A user provided description explaining the problem or reason for the service order.
        /// </summary>
        [Required]
        public string UserDescription { get; set; }

        /// <summary>
        /// Contact Details of the Person who made the order
        /// </summary>
        [Required]
        public ContactDetailsExtended OrderedBy { get; set; }

        /// <summary>
        /// Asset identifier
        /// </summary>
        [Required]
        public AssetInfo AssetInfo { get; set; }

        /// <summary>
        /// Service Provider Id
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <summary>
        /// Service Type Id. The value would one of values from <c>ServiceTypeEnum</c>
        /// </summary>
        [Required]
        public int ServiceTypeId { get; set; }

        /// <summary>
        /// Id(s) of the Service Addons or Extra Services that is/are supported by the Third party Service Provider. <para>
        /// 
        /// A <see cref="ServiceProvider"/> provides 1 or many <see cref="ServiceOrderAddon"/>. Some of them come as default. Some are selectable by Organization and some are selectable by the User.
        /// This list contains the <c>User selectable</c> service-addons.</para>
        /// </summary>
        public ISet<int> UserSelectedServiceOrderAddonIds { get; set; } = new HashSet<int>();

        public NewHardwareServiceOrderDTO(NewHardwareServiceOrder order)
        {
            DeliveryAddress = order.DeliveryAddress;
            UserDescription = order.UserDescription;
        }
    }
}
