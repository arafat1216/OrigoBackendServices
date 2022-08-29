namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     Represents the customer-specific configuration that's used with a given service-provider.
    /// </summary>
    /// <see cref="CustomerServiceProviderDto"/>
    /// <see cref="HardwareServiceOrderServices.Models.CustomerServiceProvider"/>
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
        [Obsolete("This will soon be removed in favor of the API-credential list")]
        public string? ApiUserName { get; set; }

        /// <summary>
        /// Password for call service provider's API
        /// </summary>
        [Obsolete("This will soon be removed in favor of the API-credential list")]
        public string? ApiPassword { get; set; }

        /*
         * New stuff
         */

        /// <summary>
        ///     The service-provider's identifier.
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <summary>
        ///     A list containing all API credential indicators/helpers for this <c>CustomerServiceProvider</c>. These helpers provides clues
        ///     to the frontend/APIs regarding what credentials exist, and what information is provided or missing.
        ///     
        ///     <para>
        ///     This property is only included/loaded if it's been explicitly requested. If it's not requested, the value will either
        ///     be <c><see langword="null"/></c>, or an empty list. </para>
        /// </summary>
        [SwaggerSchema(ReadOnly = true)]
        public ICollection<ObscuredApiCredential>? ApiCredentials { get; private init; }
    }
}
