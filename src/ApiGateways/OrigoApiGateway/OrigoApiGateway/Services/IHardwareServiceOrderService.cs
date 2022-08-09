
#nullable enable

namespace OrigoApiGateway.Services
{
    /// <summary>
    ///     Defines the methods and microservice-calls that should be available for hardware-services.
    /// </summary>
    public interface IHardwareServiceOrderService
    {

        /// <summary>
        ///     Retrieve all service-providers from the solution.
        /// </summary>
        /// <param name="includeSupportedServiceTypes"> If <see langword="true"/>, then the <see cref="Models.HardwareServiceOrder.Backend.ServiceProvider.SupportedServiceTypeIds">service-provider's supported service-type</see> 
        ///     list is included/loaded for all service-providers. Otherwise, the value is ignored and will be <see langword="null"/>. </param>
        /// <param name="includeOfferedServiceOrderAddons"> If <see langword="true"/>, then the <see cref="Models.HardwareServiceOrder.Backend.ServiceOrderAddon">service-provider's service-addon</see>
        ///     list is included/loaded for all service-providers. Otherwise, the value is ignored and will be <see langword="null"/>. </param>
        /// <returns> A collection that contains all service-providers that exists in the solution. </returns>
        Task<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>?> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);
    }
}
