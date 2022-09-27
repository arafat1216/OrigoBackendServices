using Common.Seedwork;
using HardwareServiceOrderServices.Models;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.ServiceModels
{
    /// <summary>
    ///     Represents a single API credential added to a given customer's service-provider configuration.
    /// </summary>
    /// <see cref="ApiCredential"/>
    public class ApiCredentialDTO
    {
        /// <summary>
        ///     Creates a new instance of the <see cref="ApiCredentialDTO"/> class.
        ///     
        ///     <para>
        ///     This is a reserved constructor intended for JSON serializers, AutoMapper, unit-testing and other automated processes. This constructor
        ///     should never be called directly in any production code. </para>
        /// </summary>
        public ApiCredentialDTO()
        {
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ApiCredentialDTO"/> class.
        /// </summary>
        /// <param name="customerServiceProviderId"> The ID for the <see cref="CustomerServiceProvider"/> that owns this API credential. </param>
        /// <param name="serviceTypeId"> The ID of the service-type this API credential is valid for. </param>
        /// <param name="apiUsername"> The API username. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        /// <param name="apiPassword"> The API password. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        public ApiCredentialDTO(int customerServiceProviderId, int? serviceTypeId, string? apiUsername, string? apiPassword)
        {
            CustomerServiceProviderId = customerServiceProviderId;
            ServiceTypeId = serviceTypeId;
            ApiUsername = apiUsername;
            ApiPassword = apiPassword;
        }


        /// <inheritdoc cref="EntityV2.Id"/>
        [JsonIgnore]
        public int Id { get; init; }

        /// <summary>
        ///     The ID for the <see cref="CustomerServiceProvider"/> that owns this API credential.
        /// </summary>
        [JsonIgnore]
        public int CustomerServiceProviderId { get; init; }

        /// <summary>
        ///     The ID of the service-type this API credential is valid for.
        ///     
        ///     <para>
        ///     <c><see cref="CustomerServiceProviderId"/></c> and <c><see cref="ServiceTypeId"/></c> is a unique combination. This means that
        ///     each <see cref="CustomerServiceProvider"/> can only contain one set of API credentials per <see cref="ServiceType"/>. </para>
        ///     
        ///     <para>
        ///     When the value is <c><see langword="null"/></c>, the key functions as the default/fallback API key. This value will be used for all 
        ///     API requests where the service-type don't have it's own specific API key.
        ///     </para>
        /// </summary>
        public int? ServiceTypeId { get; init; }

        /// <summary>
        ///     The API username. If it's not applicable for the service-provider, it should be <see langword="null"/>.
        /// </summary>
        public string? ApiUsername { get; set; }

        /// <summary>
        ///     The API password. If it's not applicable for the service-provider, it should be <see langword="null"/>.
        /// </summary>
        public string? ApiPassword { get; set; }

        /// <summary>
        ///     A timestamp stating when we last (successfully) checked and processed service-order updates using these API credentials.
        ///     If we haven't successfully fetched any updates, or the service-provider don't have a "only retrieve orders with updates after"
        ///     functionality, this will be <see langword="null"/>.
        ///     
        ///     <para>
        ///     NB: The timestamp should be for when the request was initialized, not when it was completed! </para>
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? LastUpdateFetched { get; set; }
    }
}
