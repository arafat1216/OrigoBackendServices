﻿#nullable enable

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
        ///     When customers are configuring their settings for this service-provider, 
        ///     should they add a API username when configuring their API credentials?
        /// </summary>
        [Required]
        public bool RequiresApiUsername { get; set; }

        /// <summary>
        ///     When customers are configuring their settings for this service-provider, 
        ///     should they add a API password when configuring their API credentials?
        /// </summary>
        [Required]
        public bool RequiresApiPassword { get; set; }

        /// <summary>
        ///     A list containing all service-addons that is offered by the service-provider. 
        ///     
        ///     <para>
        ///     A collection detailing the service-order addons that is offered by this service-provider.
        ///     For most requests where this list has not been explicitly requested, the value will be <c><see langword="null"/></c>.
        /// </para>
        /// </summary>
        public ICollection<ServiceOrderAddon>? OfferedServiceOrderAddons { get; set; }

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
