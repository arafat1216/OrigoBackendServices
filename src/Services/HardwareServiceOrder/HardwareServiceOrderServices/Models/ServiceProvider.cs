using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

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
        public ServiceProvider(string name, Guid organizationId, ISet<ServiceProviderServiceType> supportedServiceTypes) : base()
        {
            Name = name;
            OrganizationId = organizationId;
            SupportedServiceTypes = supportedServiceTypes;
        }

        /// <summary>
        ///     The service-provider's name.
        /// </summary>
        [MaxLength(256)]
        public string Name { get; set; }


        /// <summary>
        ///     The identifier that's used for the service-provider in it's organization-entity.
        /// </summary>
        public Guid OrganizationId { get; set; }


        /// <summary>
        ///     Retrieves all <see cref="ServiceType.Id"/> values contained in <see cref="SupportedServiceTypes"/>.
        /// </summary>
        [NotMapped]
        public IEnumerable<int> SupportedServiceTypesIds
        {
            get { return SupportedServiceTypes.Select(e => e.ServiceTypeId); }
        }

        /// <summary>
        ///     A collection of all service-types that is supported by this service-provider.
        /// </summary>
        public ICollection<ServiceProviderServiceType> SupportedServiceTypes { get; private set; }

        // TODO: Eventually, we will extend this to do something similar for supported countries and asset-categories.


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
