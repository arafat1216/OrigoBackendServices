using HardwareServiceOrderServices.Models;
using HardwareServiceOrderServices.ServiceModels;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     Indicates that the service-provider's service-class implementation supports repair services.
    /// </summary>
    public interface IRepairProvider : IGenericProviderOfferings
    {

        /// <summary>
        ///     Creates a new repair order.
        /// </summary>
        /// <param name="newRepairOrder"> The details for the new service-order. </param>
        /// <param name="serviceTypeId"> The ID of the service type that should be used. </param>
        /// <param name="serviceOrderId"> If supported by the provider, the ID we want to associate with the service order. 
        ///     For some providers this may function as an alternate key/identifier, or can be used for reference purposes. </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains the details for the newly created service-order. </returns>
        Task<NewExternalServiceOrderResponseDTO> CreateRepairOrderAsync(NewExternalServiceOrderDTO newRepairOrder, int serviceTypeId, string serviceOrderId);
    }
}
