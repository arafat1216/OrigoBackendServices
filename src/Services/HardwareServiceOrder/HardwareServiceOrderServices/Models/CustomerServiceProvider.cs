using Common.Seedwork;
using System.ComponentModel.DataAnnotations.Schema;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Mapping between customer and service provider.
    ///     
    ///     <para>
    ///     This represents one customer's configuration for a given service-provider. </para>
    /// </summary>
    public class CustomerServiceProvider : EntityV2, IDbSetEntity
    {
        /// <summary>
        ///     Constructor reserved for Entity Framework.
        /// </summary>
        public CustomerServiceProvider()
        {
        }


        /// <summary>
        ///     Initializes a new instance of the <see cref="ContactDetails"/>-class.
        /// </summary>
        /// <param name="organizationId"> The organization/customer identifier. </param>
        /// <param name="serviceProviderId"> The service-provider identifier. </param>
        /// <param name="apiCredentials"> A list of all API credentials. </param>
        /// <param name="serviceOrderAddons"> A list of all active service-order addons. </param>
        public CustomerServiceProvider(Guid organizationId, int serviceProviderId, ICollection<ApiCredential>? apiCredentials, ICollection<ServiceOrderAddon>? serviceOrderAddons)
        {
            CustomerId = organizationId;
            ServiceProviderId = serviceProviderId;
            ApiCredentials = apiCredentials ?? new List<ApiCredential>();
            ActiveServiceOrderAddons = serviceOrderAddons ?? new List<ServiceOrderAddon>();
        }


        /// <summary>
        ///     The customer ID that the configuration applies to.
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        ///     The service-provider ID that the configuration applies to.
        /// </summary>
        public int ServiceProviderId { get; set; }

        /// <summary>
        ///     Username for accessing providers APIs
        /// </summary>
        [Obsolete("This is replaced with 'ApiCredentials' and will soon be removed.")]
        public string? ApiUserName { get; set; }

        /// <summary>
        ///     Password for accessing providers APIs
        /// </summary>
        [Obsolete("This is replaced with 'ApiCredentials' and will soon be removed.")]
        public string? ApiPassword { get; set; }


        /// <summary>
        ///     Retrieve updates that has been made after this timestamp. 
        /// </summary>
        [Obsolete("This is replaced with 'ApiCredentials' and will soon be removed.")]
        public DateTimeOffset LastUpdateFetched { get; set; }


        /*
         * EF navigation properties
         */

        /// <summary>
        ///     The <see cref="Models.ServiceProvider"/> Navigation property for <see cref="Models.ServiceProvider"/>
        /// </summary>
        /// <remarks>
        ///     This is a EF navigation property, and will be <see langword="null"/> unless it's been explicitly included.
        /// </remarks>
        /// <see cref="ServiceProviderId"/>
        [ForeignKey(nameof(ServiceProviderId))]
        public virtual ServiceProvider? ServiceProvider { get; set; }


        /// <summary>
        ///     Contains the API credentials that's used with this customer-service-provider.
        /// </summary>
        /// <remarks>
        ///     This is a EF navigation property, and will be <see langword="null"/> unless it's been explicitly included.
        /// </remarks>
        public virtual ICollection<ApiCredential>? ApiCredentials { get; init; }


        /// <summary>
        ///     A list detailing all service-order addons that's currently active for this <see cref="CustomerServiceProvider"/>.
        /// </summary>
        /// <remarks>
        ///     This is a EF navigation property, and will be <see langword="null"/> unless it's been explicitly included.
        /// </remarks>
        public virtual ICollection<ServiceOrderAddon>? ActiveServiceOrderAddons { get; init; }
    }
}
