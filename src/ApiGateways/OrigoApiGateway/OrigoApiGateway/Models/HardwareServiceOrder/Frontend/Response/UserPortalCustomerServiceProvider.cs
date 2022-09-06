#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Frontend.Response
{
    /// <inheritdoc cref="Backend.Response.CustomerServiceProvider"/>
    /// <remarks>
    ///     This view-model only contains the properties and values that should be presented on the 
    ///     customer-portal's settings/configuration pages.
    /// </remarks>
    /// <see cref="Backend.Response.CustomerServiceProvider"/>
    public class UserPortalCustomerServiceProvider
    {
        /// <inheritdoc cref="Backend.Response.CustomerServiceProvider.ServiceProviderId"/>
        [Required]
        public int ServiceProviderId { get; set; }


        /// <inheritdoc cref="Backend.Response.CustomerServiceProvider.ActiveServiceOrderAddons"/>
        public virtual ICollection<UserPortalServiceOrderAddon>? ActiveServiceOrderAddons { get; set; }
    }
}
