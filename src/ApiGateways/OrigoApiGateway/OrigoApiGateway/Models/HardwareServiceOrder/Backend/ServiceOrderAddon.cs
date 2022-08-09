
#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend
{
    /// <summary>
    ///     Contains the details for a single service-addon that is offered by a <c>ServiceProvider</c>.
    /// </summary>
    public class ServiceOrderAddon
    {
        /// <summary>
        ///     The ID that uniquely identifies this service-order addon.
        /// </summary>
        [Required]
        [SwaggerSchema(ReadOnly = true)]
        public int Id { get; init; }

        /// <summary>
        ///     The ID of the <c>ServiceProvider</c> that offer the service-addon.
        /// </summary>
        [Required]
        public int ServiceProviderId { get; set; }

        /// <summary>
        ///     NB: This setting is only applicable when the service-addon is enabled/active on the customer!
        ///     
        ///     <br/><br/>
        ///     A boolean that indicates if the service-addon is enforced, or selectable by the user when placing service-orders.
        ///
        ///     <br/><br/>
        ///     - When <c><see langword="true"/></c>, the user that places the order can choose to include this service-addon when 
        ///     placing new service-orders. All user selectable service-addons are considered opt-in, and won't be included in the 
        ///     service-order unless it is explicitly added. <br/>
        ///     
        ///     - When <c><see langword="false"/></c>, the user can't choose, and service-addon is always included. 
        /// </summary>
        /// <value>
        ///     A <see cref="bool"/> that indicates if the service-addon is enforced, or selectable by the user when placing service-orders.
        /// </value>
        [Required]
        public bool IsUserSelectable { get; set; }

        /// <summary>
        ///     A boolean that indicates if customers are allowed to enable/disable this service-addon for their own organization.
        ///
        ///     <br/><br/>
        ///     - When <c><see langword="true"/></c>, then customers are able to toggle this service-addon on/off themselves in their organization settings/APIs. <br/>
        ///     
        ///     - When <c><see langword="false"/></c>, then the service-addon can only be toggled on/off in the backoffice.
        /// </summary>
        /// <value>
        ///     A <see cref="bool"/> that indicates if customers are allowed to enable/disable this service-addon for their own organization.
        /// </value>
        [Required]
        public bool IsCustomerTogglable { get; set; }
    }
}
