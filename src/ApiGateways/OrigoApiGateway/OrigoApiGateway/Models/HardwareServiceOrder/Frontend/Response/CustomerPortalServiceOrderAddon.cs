#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
    /// <remarks>
    ///     This view-model only contains the properties and values that should be presented on the 
    ///     customer-portal's settings/configuration pages.
    /// </remarks>
    /// <see cref="Backend.ServiceOrderAddon"/>
    public class CustomerPortalServiceOrderAddon
    {
        /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; init; }

        /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
        [Required]
        public bool IsUserSelectable { get; set; }
    }
}
