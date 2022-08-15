namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    ///     Represents a single API credential within a given <c><see cref="CustomerServiceProvider"/></c>.
    /// </summary>
    public class NewApiCredential
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
        ///     The API username. If it's not applicable for the service-provider, it should be <see langword="null"/>.
        ///     
        ///     <para>
        ///     This input is required if the given service-provider <see cref="ServiceProviderDTO.RequiresApiUsername">requires an API username</see>. </para>
        /// </summary>
        public string? ApiUsername { get; set; }

        /// <summary>
        ///     The API password. If it's not applicable for the service-provider, it should be <see langword="null"/>.
        ///     
        ///     <para>
        ///     This input is required if the given service-provider <see cref="ServiceProviderDTO.RequiresApiPassword">requires an API username</see>. </para>
        /// </summary>
        public string? ApiPassword { get; set; }
    }
}
