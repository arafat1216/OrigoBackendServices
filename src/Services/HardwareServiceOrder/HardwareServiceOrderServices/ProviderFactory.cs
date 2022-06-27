using HardwareServiceOrderServices.Configuration;
using HardwareServiceOrderServices.Models;
using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     A factory used for accessing service-providers based on their IDs.
    /// </summary>
    public class ProviderFactory : IProviderFactory
    {
        private readonly ServiceProviderConfiguration _providerConfiguration;

        /// <summary>
        ///     Initializes a new <see cref="ProviderFactory"/> class utilizing injections.
        /// </summary>
        /// <param name="options"> The injected <see cref="IOptions{TOptions}"/> interface. </param>
        public ProviderFactory(IOptions<ServiceProviderConfiguration> options)
        {
            _providerConfiguration = options.Value;
        }


        /// <inheritdoc/>
        public async Task<IRepairProvider> GetRepairProviderAsync(int providerId, string? apiUsername = null, string? apiPassword = null)
        {
            switch (providerId)
            {
                // Conmodo
                case 1:
                    if (string.IsNullOrEmpty(apiUsername))
                        throw new ArgumentNullException(nameof(apiUsername));
                    else
                        return GetConmodo(apiUsername);

                default:
                    throw new NotSupportedException("Repair-services is currently not supported for this service-provider.");
            }
        }


        /// <inheritdoc/>
        public async Task<IAftermarketProvider> GetAftermarketProviderAsync(int providerId, string? apiUsername = null, string? apiPassword = null)
        {
            throw new NotImplementedException("Aftermarket services is currently not supported by the solution.");

            switch (providerId)
            {
                // Add supported providers here.
                default:
                    throw new NotSupportedException("Aftermarket-services is currently not supported for this service-provider.");
            }
        }

        /// <summary>
        ///     Retrieves Conmodo's repair interface.
        /// </summary>
        /// <param name="apiUsername"> The customer's API username. This is their <see cref="CustomerSettings.ServiceId"/>. </param>
        /// <returns> The corresponding <see cref="IRepairProvider"/>. </returns>
        /// <exception cref="ApplicationException"> Thrown when the configurations/secrets was not found. </exception>
        private IRepairProvider GetConmodo(string apiUsername)
        {
            bool credentialsExist = _providerConfiguration.Providers.TryGetValue("ConmodoNO", out ProviderConfiguration conmodoConfiguration);

            if (!credentialsExist)
                throw new ApplicationException("Provider credentials was not found!");

            var provider = new Conmodo.ConmodoProviderServices(conmodoConfiguration.ApiBaseUrl, apiUsername, conmodoConfiguration.ApiPassword);
            return provider;
        }

    }
}
