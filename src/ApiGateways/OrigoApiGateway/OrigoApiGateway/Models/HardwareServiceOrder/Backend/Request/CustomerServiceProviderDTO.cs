using System;
using System.Collections.Generic;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
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
