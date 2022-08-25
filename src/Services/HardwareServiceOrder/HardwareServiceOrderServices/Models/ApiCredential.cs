using Common.Seedwork;
using System.Text.Json.Serialization;

namespace HardwareServiceOrderServices.Models
{
    /// <summary>
    ///     Represents a single API credential added to a <see cref="CustomerServiceProvider"/>.
    /// </summary>
    public class ApiCredential : EntityV2, IDbSetEntity
    {
        /// <summary>
        ///     Constructor reserved for Entity Framework.
        /// </summary>
        private ApiCredential()
        {
        }

        /// <summary>
        ///     Creates a new API credential that is ready to be inserted into the database.
        /// </summary>
        /// <param name="customerServiceProviderId"> The ID for the <see cref="CustomerServiceProvider"/> that owns this API credential. </param>
        /// <param name="serviceTypeId"> The <see cref="ServiceType.Id"/> this API credential is valid for.</param>
        /// <param name="apiUsername"> The API username. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        /// <param name="apiPassword"> The API password. If it's not applicable for the service-provider, it should be <see langword="null"/>. </param>
        public ApiCredential(int customerServiceProviderId, int serviceTypeId, string? apiUsername, string? apiPassword)
        {
            CustomerServiceProviderId = customerServiceProviderId;
            ServiceTypeId = serviceTypeId;
            ApiUsername = apiUsername;
            ApiPassword = apiPassword;
        }


        /// <summary>
        ///     The ID for the <see cref="CustomerServiceProvider"/> that owns this API credential.
        /// </summary>
        public int CustomerServiceProviderId { get; init; }

        /// <summary>
        ///     The <see cref="ServiceType.Id"/> this API credential is valid for.
        ///     
        ///     <para>
        ///     <see cref="CustomerServiceProviderId"/> and <see cref="ServiceTypeId"/> is a unique combination. This means that
        ///     each <see cref="CustomerServiceProvider"/> can only contain one set of API credentials per <see cref="ServiceType"/>. </para>
        /// </summary>
        public int ServiceTypeId { get; init; }

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
        public DateTimeOffset? LastUpdateFetched { get; set; }


        /*
         * EF navigation properties
         */

        /// <summary>
        ///     The <see cref="CustomerServiceProvider"/> that this set of API credentials are attached to.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property for <see cref="CustomerServiceProviderId"/>, and may not be 
        ///     included in the queried results.
        /// </remarks>
        /// <value>
        ///     When <see langword="null"/>, the navigation property has not been included. Otherwise, it will return the
        ///     <see cref="CustomerServiceProvider"/> the API credentials are attached to.
        /// </value>
        [JsonIgnore]
        public virtual CustomerServiceProvider? CustomerServiceProvider { get; set; }

        /// <summary>
        ///     The <see cref="ServiceType"/> that this set of API credentials are attached to.
        /// </summary>
        /// <remarks>
        ///     This is a Entity Framework navigation property for <see cref="ServiceTypeId"/>, and may not be 
        ///     included in the queried results.
        /// </remarks>
        /// <value>
        ///     When <see langword="null"/>, the navigation property has not been included. Otherwise, it will return the
        ///     <see cref="ServiceType"/> the API credentials is valid for.
        /// </value>
        [JsonIgnore]
        public virtual ServiceType? ServiceType { get; set; }
    }
}
