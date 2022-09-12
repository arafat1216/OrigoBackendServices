using Common.Seedwork;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single service-order addon that is offered by a <see cref="Models.ServiceProvider"/>.
    ///     These optional addons are products that may be included when placing service-orders in the 
    ///     service-providers external APIs or systems.
    /// </summary>
    public class ServiceOrderAddon : EntityV2, IDbSetEntity
    {
        /// <summary>
        ///     This is a reserved constructor that is used by Entity Framework. 
        ///     This constructor should not be called directly in production code!
        /// </summary>
        [Obsolete("This is a reserved constructor, and should not be called directly!")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ServiceOrderAddon() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }

        /// <summary>
        ///     Creates a new <see cref="ServiceOrderAddon"/> entity.
        /// </summary>
        /// <param name="serviceProviderId"> The <see cref="Models.ServiceProvider"/> that offers this service-order addon. </param>
        /// <param name="thirdPartyId"> The ID the service-provider uses for this addon. </param>
        public ServiceOrderAddon(int serviceProviderId, string thirdPartyId) : base()
        {
            ServiceProviderId = serviceProviderId;
            ThirdPartyId = thirdPartyId;
        }


        /// <summary>
        ///     This is a reserved constructor that is used unit-testing and data-seeding. 
        ///     This constructor should otherwise not be called directly in production code!
        /// </summary>
        /// <inheritdoc cref="EntityV2(Guid, DateTimeOffset, Guid?, DateTimeOffset?, Guid?, DateTimeOffset?, bool)"/>
        [Obsolete("This is a reserved constructor, and should not be used for anything but unit-testing and data-seeding!")]
        public ServiceOrderAddon(int id,
                                 int serviceProviderId,
                                 string thirdPartyId,
                                 bool isUserSelectable,
                                 bool isCustomerTogglable,
                                 Guid createdBy,
                                 DateTimeOffset dateCreated,
                                 Guid? updatedBy = null,
                                 DateTimeOffset? dateUpdated = null,
                                 Guid? deletedBy = null,
                                 DateTimeOffset? dateDeleted = null,
                                 bool isDeleted = false) : base(createdBy, dateCreated, updatedBy, dateUpdated, deletedBy, dateDeleted, isDeleted)
        {
            Id = id;
            ServiceProviderId = serviceProviderId;
            ThirdPartyId = thirdPartyId;
            IsUserSelectable = isUserSelectable;
            IsCustomerTogglable = isCustomerTogglable;
        }


        /*
         * Entity properties
         */


        // We only apply a override so we can include some important remarks in the docs. The logic still uses the base implementation.
        /// <inheritdoc/>
        /// <remarks>
        ///     When interacting with the service-provider's external systems, then the service-provider's identifier
        ///     should be used instead. This external identifier can be found in <see cref="ThirdPartyId"/>.
        /// </remarks>
        public override int Id { get => base.Id; protected set => base.Id = value; }

        /// <summary>
        ///     The ID this service-addon has in the service-provider's external APIs or systems.
        ///     When interacting with the external systems, this value must be used in place of our own <see cref="Id"/>.
        /// </summary>
        /// <value>
        ///     The ID the service-provider uses for this addon.
        /// </value>
        [MaxLength(256)]
        public string ThirdPartyId { get; set; }

        /// <summary>
        ///     The ID for the <see cref="Models.ServiceProvider"/> that offers this service-order addon.
        /// </summary>
        public int ServiceProviderId { get; set; }

        // TODO: Add translatable properties containing "Name" and "Description".

        /// <summary>
        ///     Is this an option that the users themselves can choose to include when they are placing service-orders?
        /// 
        ///     <para>
        ///     When <see langword="true"/>, the user that places the order may choose to include this service-addon when 
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
        ///     When <see langword="true"/>, then customers should be able to toggle the service on/off in their organization settings/APIs. <br/>
        ///     When <see langword="false"/>, it means that this is a system/backoffice-only setting. </para>
        /// </summary>
        /// <value>
        ///     A <see cref="bool"/> that indicates if customers are allowed to enable/disable this service-addon for their own organization.
        /// </value>
        public bool IsCustomerTogglable { get; set; }


        /*
         * EF navigation properties
         */

        /// <summary>
        ///     The <see cref="Models.ServiceProvider"/> that offers this service-order addon.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results.
        /// </remarks>
        [ForeignKey(nameof(ServiceProviderId))]
        public virtual ServiceProvider? ServiceProvider { get; set; }


        /// <summary>
        ///     A list of all <see cref="CustomerServiceProvider"/>s that uses the service-order addon.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results.
        /// </remarks>
        public virtual ICollection<CustomerServiceProvider>? CustomerServiceProviders { get; set; }
    }
}
