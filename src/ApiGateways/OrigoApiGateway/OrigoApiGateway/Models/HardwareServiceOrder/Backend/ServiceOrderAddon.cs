#nullable enable

using OrigoApiGateway;

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
        ///     Is this an option that the users themselves can choose to include when they are placing service-orders?
        /// 
        ///     <para>
        ///     When <c><see langword="true"/></c>, the user that places the order may choose to include this service-addon when 
        ///     creating a new service-order. All user selectable service-addons are opt-in, and is not included in the service-order
        ///     unless the user has explicitly added it to the order. </para>
        ///     
        ///     <para>
        ///     If <see langword="false"/>, then the service-addon is always included. </para>
        /// </summary>
        /// <remarks>
        ///     NB: This setting is only applicable if the service-addon has been enabled on the customer!
        /// </remarks>
        /// <value>
        ///     A <see cref="bool"/> that indicates if the service-addon is enforced, or selectable by the user when placing service-orders.
        /// </value>
        public bool IsUserSelectable { get; set; }

        /// <summary>
        ///     Can customers turn this addon on/off for their own organization using the administration APIs?
        /// 
        ///     <para>
        ///     When <see langword="true"/>, then customers should be able to toggle the service on/off in their organization settings/APIs. </para>
        ///     
        ///     <para>
        ///     When <see langword="false"/>, it means that this is a system/backoffice-only setting. </para>
        /// </summary>
        /// <value>
        ///     A <see cref="bool"/> that indicates if customers are allowed to enable/disable this service-addon for their own organization.
        /// </value>
        public bool IsCustomerTogglable { get; set; }
    }
}
