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
        /// </summary>
        [Required]
        public int ServiceTypeId { get; init; }

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
            if (string.IsNullOrWhiteSpace(ApiUsername) && string.IsNullOrWhiteSpace(ApiPassword))
            {
                yield return new ValidationResult("No valid username and/or password were provided.",
                                                  new[] { nameof(ApiUsername), nameof(ApiPassword) });
            }
        }
    }
}
