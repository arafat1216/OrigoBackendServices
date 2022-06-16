using Microsoft.AspNetCore.Mvc;

namespace HardwareServiceOrder.API.ViewModels
{
    public class CustomerServiceProvider
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
