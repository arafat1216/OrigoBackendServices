using System;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request
{
    /// <summary>
    /// Creates new Hardware Service Order.
    /// </summary>
    public class NewHardwareServiceOrder
    {
        /// <summary>
        /// Delivery address of the order
        /// </summary>
        [Required]
        public DeliveryAddress DeliveryAddress { get; set; }
        
        /// <summary>
        /// Fault description
        /// </summary>
        [Required]
        public string ErrorDescription { get; set; }
        
        /// <summary>
        /// Asset identifier
        /// </summary>
        [Required]
        public Guid AssetId { get; set; }
        
        /// <summary>
        /// Service Provider Id
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <summary>
        /// Id(s) of the Service Addons or Extra Services that is/are supported by the Third party Service Provider. <para>
        /// 
        /// A <see cref="ServiceProvider"/> provides 1 or many Service Order Addons<see cref="Backend.ServiceOrderAddon"/>. Some of them come as default. Some are selectable by Organization and some are selectable by the User.
        /// This list contains the <c>User selectable</c> service-addons.</para>
        /// </summary>
        public ISet<int> UserSelectedServiceOrderAddonIds { get; set; } = new HashSet<int>();
    }
}
