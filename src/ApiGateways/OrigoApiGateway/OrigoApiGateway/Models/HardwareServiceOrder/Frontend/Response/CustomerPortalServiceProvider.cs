#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    /// <inheritdoc cref="Backend.ServiceProvider"/>
    /// <remarks>
    ///     This view-model only contains the properties and values that should be presented on the 
    ///     customer-portal's settings/configuration pages.
    /// </remarks>
    /// <see cref="Backend.ServiceProvider"/>
    public class CustomerPortalServiceProvider
    {
        /// <summary>
        ///     The service-provider's identifier.
        /// </summary>
        /// <value>
        ///     The service-provider's identifier.
        /// </value>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; init; }

        /// <summary>
        ///     The service-provider's name (non-localized).
        /// </summary>
        [MaxLength(256)]
        [Required]
        public string Name { get; set; } = null!;

        /// <summary>
        ///     A list containing all service-addons that is offered by the service-provider. 
        ///     
        ///     <para>
        ///     A collection detailing the service-order addons that is offered by this service-provider.
        ///     For most requests where this list has not been explicitly requested, the value will be <c><see langword="null"/></c>. </para>
        /// </summary>
        public ICollection<CustomerPortalServiceOrderAddon>? OfferedServiceOrderAddons { get; set; }

        /// <summary>
        ///     A set containing the IDs for all service-types that's supported by the service-provider.
        /// </summary>
        /// <value>
        ///     A set detailing the service-type IDs that can be used with this service-provider.
        ///     For most requests where this list has not been explicitly requested, the value will be <c><see langword="null"/></c>.
        /// </value>
        public ISet<int>? SupportedServiceTypeIds { get; set; }
    }
}
