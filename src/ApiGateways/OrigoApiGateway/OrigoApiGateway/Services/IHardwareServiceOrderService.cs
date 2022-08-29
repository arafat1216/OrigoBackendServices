using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Request;
using OrigoApiGateway.Models.HardwareServiceOrder.Backend.Response;

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
        Task<IEnumerable<Models.HardwareServiceOrder.Backend.ServiceProvider>> GetAllServiceProvidersAsync(bool includeSupportedServiceTypes, bool includeOfferedServiceOrderAddons);


        /// <summary>
        ///     Retrieves all <see cref="CustomerServiceProvider"/>-configurations that's used by a given customer.
        /// </summary>
        /// <param name="organizationId"> The customers identifier. </param>
        /// <param name="includeApiCredentialIndicators"> Should <see cref="CustomerServiceProvider.ApiCredentials"/> be loaded and included in the results? </param>
        /// <returns> A task that represents the asynchronous operation. The task result contains a list of all retrieved customer-service-provider details. </returns>
        Task<IEnumerable<CustomerServiceProvider>> GetCustomerServiceProvidersAsync(Guid organizationId, bool includeApiCredentialIndicators);

        /// <summary>
        ///     Deletes an existing API credential.
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="serviceTypeId"> The service-type the API credentials can be used with. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task DeleteApiCredentialsAsync(Guid organizationId, int serviceProviderId, int serviceTypeId);

        /// <summary>
        ///     Adds or updates an API credential for a customer's service-provider configuration.
        ///     
        ///     <para>
        ///     If an existing credential already exist (using the same unique combination of <paramref name="organizationId"/>, 
        ///     <paramref name="serviceProviderId"/> and <see cref="NewApiCredential.ServiceTypeId"/>), then it will be overwritten using the new values. </para>
        /// </summary>
        /// <param name="organizationId"> The customer the API credential is attached to. </param>
        /// <param name="serviceProviderId"> The service-provider the API credential is used with. </param>
        /// <param name="apiCredential"> The API credential details to add or update. </param>
        /// <returns> A task that represents the asynchronous operation. </returns>
        Task AddOrUpdateApiCredentialAsync(Guid organizationId, int serviceProviderId, NewApiCredential apiCredential);
    }
}
