namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     Represents a single API credential that's added to a customer's service-provider configuration (<c><see cref="CustomerServiceProvider"/></c>).
    ///     For security reasons, the actual API credential values is not exposed. We only provide details on weather or not a value has been provided. 
    ///     
    ///     <br/><br/>
    ///     The purpose of this obscured information is allow the frontend / external APIs to see what API credential's that's configured,
    ///     and what's been provided in them (can be used for validation and error checking).
    /// </summary>
    /// <see cref="ApiCredentialDTO"/>
    [SwaggerSchema(ReadOnly = true)]
    public class ObscuredApiCredential
    {
        /// <summary>
        ///     The ID of the service-type this API credential is valid for.
        /// </summary>
        [Required]
        public int ServiceTypeId { get; init; }

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
