using System;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Request
{
    public class NewHardwareServiceOrder
    {
        /// <summary>
        /// Delivery address of the order
        /// </summary>
        public DeliveryAddress DeliveryAddress { get; set; }
        /// <summary>
        /// Fault description
        /// </summary>
        public string ErrorDescription { get; set; }
        /// <summary>
        /// Asset identifier
        /// </summary>
        public Guid AssetId { get; set; }
    }
}
