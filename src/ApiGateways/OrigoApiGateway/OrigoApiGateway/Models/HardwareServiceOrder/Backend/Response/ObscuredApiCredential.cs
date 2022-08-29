#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response
{
    /// <summary>
    ///     Represents a single API credential that's added to a customer's service-provider configuration (<c><see cref="CustomerServiceProvider"/></c>).
    ///     For security reasons, the actual API credential values is not exposed. We only provide details on weather or not a value has been provided. 
    ///     
    ///     <br/><br/>
    ///     The purpose of this obscured information is allow the frontend / external APIs to see what API credential's that's configured,
    ///     and what's been provided in them (can be used for validation and error checking).
    /// </summary>
    [SwaggerSchema(ReadOnly = true)]
    public class ObscuredApiCredential
    {
        /// <summary>
        ///     The ID of the service-type this API credential is valid for.
        ///     
        ///     <para>
        ///     When the value is <c><see langword="null"/></c>, the key functions as the default/fallback API key. This value will be used for all 
        ///     API requests where the service-type don't have it's own specific API key.
        ///     </para>
        /// </summary>
        [Required]
        public int? ServiceTypeId { get; set; }

        /// <summary>
        ///     This is <c><see langword="true"/></c> when a API username (or another corresponding value) has been provided. 
        ///     If no values has been stored, the value will be <c><see langword="false"/></c>.
        /// </summary>
        [Required]
        public bool ApiUsernameFilled { get; set; }

        /// <summary>
        ///     This is <c><see langword="true"/></c> when a API password (or another corresponding value) has been provided. 
        ///     If no values has been stored, the value will be <c><see langword="false"/></c>.
        /// </summary>
        [Required]
        public bool ApiPasswordFilled { get; set; }
    }
}
