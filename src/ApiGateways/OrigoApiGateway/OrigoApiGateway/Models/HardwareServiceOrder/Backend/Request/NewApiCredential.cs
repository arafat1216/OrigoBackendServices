using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request
{
    /// <summary>
    ///     Represents a single API credential within a given <c><see cref="CustomerServiceProvider"/></c>.
    /// </summary>
    public class NewApiCredential : IValidatableObject
    {
        /// <summary>
        ///     The ID of the service-type this API credential is valid for.
        ///     
        ///     <para>
        ///     A <c><see cref="CustomerServiceProvider"/></c> can only have a single API credential per service-type. </para>
        ///     
        ///     <para>
        ///     When the value is <c><see langword="null"/></c>, the key functions as the default/fallback API key. This value will be used for all 
        ///     API requests where the service-type don't have it's own specific API key.
        ///     </para>
        /// </summary>
        public int? ServiceTypeId { get; init; }

        /// <summary>
        ///     The API username. If it's not applicable for the service-provider, it should be <c><see langword="null"/></c>.
        ///     
        ///     <para>
        ///     This input is required if the given service-provider <see cref="ServiceProvider.RequiresApiUsername">requires an API username</see>. </para>
        /// </summary>
        public string? ApiUsername { get; set; }

        /// <summary>
        ///     The API password. If it's not applicable for the service-provider, it should be <c><see langword="null"/></c>.
        ///     
        ///     <para>
        ///     This input is required if the given service-provider <see cref="ServiceProvider.RequiresApiPassword">requires an API username</see>. </para>
        /// </summary>
        public string? ApiPassword { get; set; }


        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Ensure that at least one of the API credential values is provided (username or password).
            if (string.IsNullOrWhiteSpace(ApiUsername) && string.IsNullOrWhiteSpace(ApiPassword))
            {
                yield return new ValidationResult("No valid username and/or password were provided.",
                                                  new[] { nameof(ApiUsername), nameof(ApiPassword) });
            }

            // If a username is provided, ensure it's valid. The placeholder value from Swagger, as well as empty strings should not be allowed!
            if (ApiUsername is not null && (string.IsNullOrEmpty(ApiUsername) || ApiUsername.ToLowerInvariant().Equals("string")))
            {
                yield return new ValidationResult("A invalid/placeholder value has been submitted. Please provide a proper value.",
                                                  new[] { nameof(ApiUsername) });
            }

            // If a password is provided, ensure it's valid. The placeholder value from Swagger, as well as empty strings should not be allowed!
            if (ApiPassword is not null && (string.IsNullOrEmpty(ApiPassword) || ApiPassword.ToLowerInvariant().Equals("string")))
            {
                yield return new ValidationResult("A invalid/placeholder value has been submitted. Please provide a proper value.",
                                                  new[] { nameof(ApiPassword) });
            }

            // Explicitly reject the 'null' enum value, as it's considered 'valid' by the enum check.
            if (ServiceTypeId == 0)
            {
                yield return new ValidationResult("An invalid/placeholder value has been submitted. Please provide a proper value.",
                                                  new[] { nameof(ServiceTypeId) });
            }
        }

    }
}
