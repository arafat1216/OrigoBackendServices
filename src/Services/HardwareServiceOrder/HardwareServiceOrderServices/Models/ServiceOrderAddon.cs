using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single service-order addon that is offered by a <see cref="Models.ServiceProvider"/>.
    ///     These optional addons are products that may be included when placing service-orders in the 
    ///     service-providers external APIs or systems.
    /// </summary>
    public class ServiceOrderAddon : EntityV2
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
    }
}
