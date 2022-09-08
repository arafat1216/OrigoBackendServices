using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     Indicates that the provider's service-class implementation supports repair services.
    /// </summary>
    public interface IRepairProvider
    {

        /// <summary>
        ///     Creates a new repair order.
        /// </summary>
        /// <param name="newRepairOrder"> The repair order details. </param>
        /// <param name="serviceTypeId"> The ID of the service type that should be used. </param>
        /// <param name="serviceOrderId"> If supported by the provider, the ID we want to associate with the service order. 
        ///     For some providers this may function as an alternate key/identifier, or can be used for reference purposes. </param>
        /// <returns> The registered repair order. </returns>
        Task<NewExternalServiceOrderResponseDTO> CreateRepairOrderAsync(NewExternalServiceOrderDTO newRepairOrder, int serviceTypeId, string serviceOrderId);

        /// <summary>
        ///     Retrieve a specific repair order using it's ID(s). Based on the service-provider, one or several keys may be required.
        /// </summary>
        /// <param name="serviceProviderOrderId1"> The service-providers primary order ID. </param>
        /// <param name="serviceProviderOrderId2"> This is not used by all service-providers, and it's use can vary between providers.
        ///     If the value is not used by the service-provider, is can be set to <c><see langword="null"/></c>. <para>
        ///     
        ///     Typically this is used for alternate IDs, but some providers requires composite-keys (2 identifiers) 
        ///     for retrieving service-orders. </para></param>
        /// <returns></returns>
        Task<ExternalServiceOrderDTO> GetRepairOrderAsync(string serviceProviderOrderId1, string? serviceProviderOrderId2);

        /// <summary>
        ///     Checks for updated repair orders, and returns a list of all orders that has been updated after the provided date.
        /// </summary>
        /// <param name="since"> Only retrieve updates that has been made after this timestamp. </param>
        /// <returns> All repair orders that contains updates. </returns>
        Task<IEnumerable<ExternalServiceOrderDTO>> GetUpdatedRepairOrdersAsync(DateTimeOffset since);
    }
}
