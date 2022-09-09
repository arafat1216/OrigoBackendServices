using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     Defines functionality that a service-provider's service-class implementation shares across all service-types.
    /// </summary>
    public interface IGenericProviderOfferings
    {
        /// <summary>
        ///     Retrieve a specific service-order using it's ID(s). Based on the service-provider, one or several keys may be required.
        /// </summary>
        /// <param name="serviceProviderOrderId1"> The service-providers primary order ID. </param>
        /// <param name="serviceProviderOrderId2"> This is not used by all service-providers, and it's use can vary between providers.
        ///     If the value is not used by the service-provider, is can be set to <c><see langword="null"/></c>. <para>
        ///     
        ///     Typically this is used for alternate IDs, but some providers requires composite-keys (2 identifiers) 
        ///     for retrieving service-orders. </para></param>
        /// <returns></returns>
        Task<ExternalServiceOrderDTO> GetOrderByIdAsync(string serviceProviderOrderId1, string? serviceProviderOrderId2);

        /// <summary>
        ///     Checks for updated service-orders, and returns a list of all orders that has been updated after the provided date.
        /// </summary>
        /// <param name="since"> Only retrieve updates that has been made after this timestamp. </param>
        /// <returns> All repair orders that contains updates. </returns>
        Task<IEnumerable<ExternalServiceOrderDTO>> GetOrdersUpdatedSinceAsync(DateTimeOffset since);
    }
}
