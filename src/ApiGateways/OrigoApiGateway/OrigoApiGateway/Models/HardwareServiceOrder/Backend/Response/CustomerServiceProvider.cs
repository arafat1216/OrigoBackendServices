#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response
{
    /// <summary>
    ///     Represents the customer-specific configuration that's used with a given service-provider.
    /// </summary>
    public class CustomerServiceProvider
    {
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
        public ICollection<ObscuredApiCredential>? ApiCredentials { get; set; }

        /// <summary>
        ///     A list containing all service-order addons that's currently active for this <c>CustomerServiceProvider</c>.
        ///     
        ///     <para>
        ///     This property is only included/loaded if it's been explicitly requested. If it's not requested, the value will either
        ///     be <c><see langword="null"/></c>, or an empty list. </para>
        /// </summary>
        public virtual ICollection<ServiceOrderAddon>? ActiveServiceOrderAddons { get; set; }
    }
}
