using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     Indicates that the service-provider's service-class implementation supports aftermarket services.
    /// </summary>
    public interface IAftermarketProvider : IGenericProviderOfferings
    {
        /// <summary>
        ///     Creates a new aftermarket-order.
        /// </summary>
        /// <param name="newAftermarketOrder"> The details for the new service-order. </param>
        /// <param name="serviceTypeId"> The ID of the service type that should be used. </param>
        /// <param name="serviceOrderId"> If supported by the provider, the ID we want to associate with the service order. 
        ///     For some providers this may function as an alternate key/identifier, or can be used for reference purposes. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the details for the newly created service-order. </returns>
        Task<NewExternalServiceOrderResponseDTO> CreateAftermarketOrderAsync(NewExternalServiceOrderDTO newAftermarketOrder, int serviceTypeId, string serviceOrderId);
    }
}
