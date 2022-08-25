using Common.Seedwork;
using HardwareServiceOrderServices.Models;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents a single customer's configuration/settings targeting a specific service-provider.
    /// </summary>
    /// <remarks>
    ///     The two properties <see cref="OrganizationId"/> and <see cref="ServiceProviderId"/> act as an alternate (composite) key for this entity,
    ///     and may be used in place of <see cref="Id"/>.
    /// </remarks>
    public class CustomerServiceProviderDto
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="CustomerServiceProviderDto"/> class.
        ///     
        ///     <para>
        ///     This is a reserved constructor intended for AutoMapper, unit-testing and other automated processes. This constructor should
        ///     never be called directly in any production code. </para>
        /// </summary>
        public CustomerServiceProviderDto()
        {
        }


        public CustomerServiceProviderDto(Guid organizationId, int serviceProviderId)
        {
            OrganizationId = organizationId;
            ServiceProviderId = serviceProviderId;
        }


        /// <summary>
        ///     Used for creating a new service-order after it's been registered in the service-provider's system.
        /// </summary>
        /// <remarks>
        ///     <b>This is a reserved constructor that is only intended for use with unit-testing.</b>
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="organizationId"></param>
        /// <param name="serviceProviderId"></param>
        /// <param name="apiCredentials"></param>
        public CustomerServiceProviderDto(int id, Guid organizationId, int serviceProviderId, ICollection<ApiCredentialDTO>? apiCredentials)
        {
            Id = id;
            OrganizationId = organizationId;
            ServiceProviderId = serviceProviderId;
            ApiCredentials = apiCredentials;
        }


        /// <inheritdoc cref="EntityV2.Id"/>
        [JsonIgnore]
        public int Id { get; private init; }

        /// <inheritdoc cref="CustomerServiceProvider.CustomerId"/>
        public Guid OrganizationId { get; set; }

        /// <inheritdoc cref="CustomerServiceProvider.ServiceProviderId"/>
        public int ServiceProviderId { get; set; }

        /// <inheritdoc cref="CustomerServiceProvider.ApiCredentials"/>
        public virtual ICollection<ApiCredentialDTO>? ApiCredentials { get; private init; }


        /*
         * EF navigation properties
         */

        /// <inheritdoc cref="CustomerServiceProvider.ServiceProvider"/>
        [JsonIgnore]
        public virtual ServiceProvider? ServiceProvider { get; private init; }
    }
}
