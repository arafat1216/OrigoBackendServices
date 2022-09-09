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
        /// Id(s) of the Service Addons or Extra Services that is/are supported by the Service Provider.
        /// </summary>
        public ISet<int> ServiceOrderAddons { get; set; } = new HashSet<int>();
    }
}
