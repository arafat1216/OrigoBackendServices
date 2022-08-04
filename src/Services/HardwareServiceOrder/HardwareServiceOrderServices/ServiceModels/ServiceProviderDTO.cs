using HardwareServiceOrderServices.Models;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <inheritdoc cref="ServiceProvider"/>
    public class ServiceProviderDTO
    {
        /// <summary>
        ///     The service-provider's identifier.
        /// </summary>
        /// <value>
        ///     The service-provider's identifier.
        /// </value>
        public int Id { get; init; }

        /// <inheritdoc cref="ServiceProvider.Name"/>
        [MaxLength(256)]
        public string Name { get; set; }

        /// <inheritdoc cref="ServiceProvider.OrganizationId"/>
        public Guid OrganizationId { get; set; }


        /*
         * Entity Framework navigation properties
         */
        
        /// <inheritdoc cref="ServiceProvider.SupportedServiceTypes"/>
        [JsonIgnore]
        public virtual ICollection<ServiceProviderServiceType>? SupportedServiceTypes { get; private set; }

        /// <inheritdoc cref="ServiceProvider.OfferedServiceOrderAddons"/>
        public virtual ICollection<ServiceOrderAddonDTO>? OfferedServiceOrderAddons { get; private set; }


        /*
         * Misc / new properties
         */

        /// <summary>
        ///     A collection of all service-type IDs that is supported by this service-provider.
        /// </summary>
        /// <remarks>
        ///     This may not be included in the queried results.
        /// </remarks>
        /// <value>
        ///     When <see langword="null"/>, the property has not been included. Otherwise, it will return a collection
        ///     detailing the service-type IDs can be used with this service-provider.
        /// </value>
        public IEnumerable<int>? SupportedServiceTypeIds
        {
            get
            {
                var result = SupportedServiceTypes?.Select(e => e.ServiceTypeId).Distinct();
                return result;
            }
        }
    }
}
