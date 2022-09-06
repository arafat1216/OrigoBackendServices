using Common.Seedwork;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single service-provider.
    /// </summary>
    public class ServiceProvider : EntityV2, IAggregateRoot, IDbSetEntity
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
        /// <param name="requiresApiUsername"> When customers are using this service-provider, do they need to add a <see cref="ApiCredential.ApiUsername">API username</see>? </param>
        /// <param name="requiresApiPassword"> When customers are using this service-provider, do they need to add a <see cref="ApiCredential.ApiPassword">API password</see>? </param>
        public ServiceProvider(string name, Guid organizationId, bool requiresApiUsername, bool requiresApiPassword) : base()
        {
            Name = name;
            OrganizationId = organizationId;
        }


        /// <summary>
        ///     This is a reserved constructor that is used unit-testing and data-seeding. 
        ///     This constructor should otherwise not be called directly in production code!
        /// </summary>
        /// <inheritdoc cref="EntityV2(Guid, DateTimeOffset, Guid?, DateTimeOffset?, Guid?, DateTimeOffset?, bool)"/>
        public ServiceProvider(int id,
                               string name,
                               Guid organizationId,
                               bool requiresApiUsername, 
                               bool requiresApiPassword,
                               ICollection<ServiceProviderServiceType>? supportedServiceTypes,
                               ICollection<ServiceOrderAddon>? offeredServiceOrderAddons,
                               Guid createdBy,
                               DateTimeOffset dateCreated,
                               Guid? updatedBy = null,
                               DateTimeOffset? dateUpdated = null,
                               Guid? deletedBy = null,
                               DateTimeOffset? dateDeleted = null,
                               bool isDeleted = false) : base(createdBy, dateCreated, updatedBy, dateUpdated, deletedBy, dateDeleted, isDeleted)
        {
            Id = id;
            Name = name;
            OrganizationId = organizationId;
            RequiresApiUsername = requiresApiUsername;
            RequiresApiPassword = requiresApiPassword;
            SupportedServiceTypes = supportedServiceTypes;
            OfferedServiceOrderAddons = offeredServiceOrderAddons;
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


        /// <summary>
        ///     When this service-provider is used in a <see cref="CustomerServiceProvider"/>, are the <see cref="ApiCredential.ApiUsername"/> relevant?
        ///     
        ///     <para>
        ///     If <see langword="true"/>, the username should be provided. Otherwise it's not used and should be <see langword="null"/>. </para>
        /// </summary>
        /// <value>
        ///     When customers are using this service-provider, do they need to add a <see cref="ApiCredential.ApiUsername">API username</see>?
        /// </value>
        public bool RequiresApiUsername { get; set; }

        /// <summary>
        ///     When this service-provider is used in a <see cref="CustomerServiceProvider"/>, are the <see cref="ApiCredential.ApiPassword"/> relevant?
        ///     
        ///     <para>
        ///     If <see langword="true"/>, the password should be provided. Otherwise it's not used and should be <see langword="null"/>. </para>
        /// </summary>
        /// <value>
        ///     When customers are using this service-provider, do they need to add a <see cref="ApiCredential.ApiPassword">API password</see>?
        /// </value>
        public bool RequiresApiPassword { get; set; }


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
