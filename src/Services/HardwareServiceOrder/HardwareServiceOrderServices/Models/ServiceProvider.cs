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
        public ServiceProvider(Guid organizationId, string name) : base()
        {
            OrganizationId = organizationId;
            Name = name;
        }


        /// <summary>
        ///     The identifier that's used for the service-provider in it's organization-entity.
        /// </summary>
        public Guid OrganizationId { get; set; }

        /// <summary>
        ///     The service-provider's name.
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; }


        // TODO: Add misc. provider specific things here in the future (e.g. supported categories, models, countries, etc.)...


        /*
         * Entity Framework navigation properties
         */

        /// <summary>
        ///     All customer-configurations that's specific for this service-provider.
        ///     
        ///     <para>
        ///     This is a Entity Framework navigation property, and may not be included in the queried results. </para>
        /// </summary>
        public virtual ICollection<CustomerServiceProvider>? CustomerServiceProviders { get; set; }

    }
}
