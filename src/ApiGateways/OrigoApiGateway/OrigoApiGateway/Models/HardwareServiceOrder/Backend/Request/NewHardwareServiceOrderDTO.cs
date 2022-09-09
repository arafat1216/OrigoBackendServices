using OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request;
using System;

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
        public DeliveryAddress DeliveryAddress { get; set; }
        
        /// <summary>
        /// Fault description
        /// </summary>
        public string ErrorDescription { get; set; }
        
        public ContactDetailsExtended OrderedBy { get; set; }
        
        /// <summary>
        /// Asset identifier
        /// </summary>
        [Required]
        public AssetInfo AssetInfo { get; set; }
        
        /// <summary>
        /// Service Provider Id
        /// </summary>
        public int ServiceProviderId { get; set; }

        /// <summary>
        /// Service Type Id. The value would one of values from <c>ServiceTypeEnum</c>
        /// </summary>
        public int ServiceTypeId { get; set; }
        
        /// <summary>
        /// Id(s) of the Service Addons or Extra Services that is/are supported by the Service Provider.
        /// </summary>
        public ISet<int> ServiceOrderAddons { get; set; } = new HashSet<int>();

        public NewHardwareServiceOrderDTO(NewHardwareServiceOrder order)
        {
            DeliveryAddress = order.DeliveryAddress;
            ErrorDescription = order.ErrorDescription;
        }
    }
}
