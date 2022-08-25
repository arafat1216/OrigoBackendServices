namespace HardwareServiceOrder.API.ViewModels
{
    public class CustomerServiceProvider
    {
        /// <summary>
        /// Provider identifier
        /// </summary>
        [Obsolete("This is replaced with 'ServiceProviderId'.")]
        public int ProviderId { get; set; }

        /// <summary>
        /// Username for calling service provider's API
        /// </summary>
        [Obsolete("This will soon be removed in favour of the api credential list")]
        public string? ApiUserName { get; set; }

        /// <summary>
        /// Password for call service provider's API
        /// </summary>
        [Obsolete("This will soon be removed in favour of the api credential list")]
        public string? ApiPassword { get; set; }

        /*
         * New stuff
         */

        [Required]
        public int ServiceProviderId { get; set; }

        [SwaggerSchema(ReadOnly = true)]
        public ICollection<ViewModels.ObscuredApiCredential>? ApiCredentials { get; private init; }
    }
}
