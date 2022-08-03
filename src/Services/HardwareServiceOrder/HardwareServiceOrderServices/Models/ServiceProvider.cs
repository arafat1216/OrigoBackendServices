using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single service-provider.
    /// </summary>
    public class ServiceProvider : EntityV2, IAggregateRoot
    {
        /// <summary>
        ///     This is a reserved constructor that is used by Entity Framework. 
        ///     This constructor should not be called directly in production code!
        /// </summary>
        [Obsolete("This is a reserved constructor, and should not be called directly!")]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private ServiceProvider() : base()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
        }


        /// <summary>
        ///     Creates a new <see cref="ServiceProvider"/> entity.
        /// </summary>
        /// <param name="organizationId"> The identifier that's used for the service-provider in it's organization-entity. </param>
        /// <param name="name"> The service provider's name. </param>
        public ServiceProvider(string name, Guid organizationId) : base()
        {
            Name = name;
            OrganizationId = organizationId;
        }

        /// <summary>
        ///     The service-provider's name.
        /// </summary>
        /// <value>
        ///     The service-provider's name.
        /// </value>
        [MaxLength(256)]
        public string Name { get; set; }


        /// <summary>
        ///     The system-specific identifier that is used for identifying or retrieving the service-provider across microservices.
        ///     
        ///     This identifier is created and maintained by the microservice that's responsible for handling all the 
        ///     organizations/customers, users, etc. for the entire system.
        /// </summary>
        /// <value>
        ///     The service-provider's organization-ID.
        /// </value>
        public Guid OrganizationId { get; set; }



        // TODO: Eventually, we will extend this to do something similar for supported countries and asset-categories.


        /*
         * Entity Framework navigation properties
         */

        /// <summary>
        ///     A collection of all service-types that is supported by this service-provider.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results.
        /// </remarks>
        /// <value>
        ///     When <see langword="null"/>, the navigation property has not been included. Otherwise, it will return a collection
        ///     detailing the service-types can be used with this service-provider.
        /// </value>
        public virtual ICollection<ServiceProviderServiceType>? SupportedServiceTypes { get; private set; }

        /// <summary>
        ///     A collection of all service-order addons that's offered by this service-provider.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results.
        /// </remarks>
        /// <value>
        ///     When <see langword="null"/>, the navigation property has not been included. Otherwise, it will return a collection
        ///     detailing the service-order addons that is offered by this service-provider.
        /// </value>
        public virtual ICollection<ServiceOrderAddon>? OfferedServiceOrderAddons { get; private set; }

        /// <summary>
        ///     A collection containing all customer-configurations that's specific for this service-provider.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results.
        /// </remarks>
        /// <value>
        ///     When <see langword="null"/>, the navigation property has not been included. Otherwise, it will return a collection
        ///     containing the <see cref="CustomerServiceProvider"/> entities tied to this service-provider.
        /// </value>
        public virtual ICollection<CustomerServiceProvider>? CustomerServiceProviders { get; private set; }

    }
}
