#nullable enable

using OrigoApiGateway.Models.HardwareServiceOrder.Backend;

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend
{
    /// <summary>
    ///     Contains information about a single service-provider.
    /// </summary>
    public class ServiceProvider
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
        ///     The ID for the service-provider's <c><see cref="Organization"/></c> entity.
        /// </summary>
        [Required]
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     A list containing all service-addons that is offered by the service-provider. 
        ///     
        ///     <para>
        ///     If <c><see langword="null"/></c>, then the has not been explicitly requested. 
        ///     Otherwise, it will return a collection detailing the service-order addons that is offered by this service-provider. </para>
        /// </summary>
        public ICollection<ServiceOrderAddon>? OfferedServiceOrderAddons { get; set; }

        /// <summary>
        ///     A set containing the IDs for all service-types that's supported by the service-provider.
        /// </summary>
        /// <value>
        ///     If <c><see langword="null"/></c>, then the has not been explicitly requested. 
        ///     Otherwise, it will return a set detailing the service-type IDs that can be used with this service-provider.
        /// </value>
        public ISet<int>? SupportedServiceTypeIds { get; set; }
    }
}
