using System;
using System.Collections.Generic;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    [Obsolete("This is used with the older HW Repair endpoints, and will soon be deprecated.")]
    public class CustomerServiceProviderDTO
    {
        /// <summary>
        /// Provider identifier
        /// </summary>
        public int ProviderId { get; set; }

        /// <summary>
        /// Username for calling service provider's API
        /// </summary>
        public string? ApiUserName { get; set; }

        /// <summary>
        /// Password for call service provider's API
        /// </summary>
        public string? ApiPassword { get; set; }
    }
}
