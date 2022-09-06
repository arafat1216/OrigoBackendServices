#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
    /// <remarks>
    ///     This view-model only contains the properties and values that should be presented in the user-portal.
    /// </remarks>
    /// <see cref="Backend.ServiceOrderAddon"/>
    public class UserPortalServiceOrderAddon
    {
        /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; init; }

        /// <inheritdoc cref="Backend.ServiceOrderAddon"/>
        [Required]
        public int ServiceProviderId { get; set; }
    }
}

