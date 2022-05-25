using Microsoft.Extensions.Options;

namespace HardwareServiceOrderServices
{
    /// <summary>
    ///     A factory used for accessing service-providers based on their IDs.
    /// </summary>
    public class ProviderFactory
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


        /// <summary>
        ///     Retrieve a given service-provider's <see cref="IRepairProvider"/> implementation that will be used for all repair-requests towards the provider.
        /// </summary>
        /// <param name="providerId"> The ID of the service provider. </param>
        /// <param name="apiUsername"> If required by the service-provider; the customer-specific username used for all API-requests.
        ///     If the service-provider don't require a customer-specific username, then this can be <see langword="null"/>. </param>
        /// <param name="apiPassword"> If required by the service-provider; the customer-specific password used for all API-requests.
        ///     If the service-provider don't require a customer-specific password, then this can be <see langword="null"/>. </param>
        /// <returns> The <see cref="IRepairProvider"/> for the given service-provider. </returns>
        /// <exception cref="ArgumentNullException"> Thrown if <paramref name="apiPassword"/> or <paramref name="apiUsername"/> is required by
        ///     the service-provider, but the values has not been provided. </exception>
        /// <exception cref="NotSupportedException"> Thrown if the provided <paramref name="providerId"/> is unsupported. </exception>
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
        /// <param name="apiUsername"> The customer's API username. </param>
        /// <returns> The corresponding <see cref="IRepairProvider"/>. </returns>
        /// <exception cref="ApplicationException"> Thrown when the configurations/secrets was not found. </exception>
        private IRepairProvider GetConmodo(string apiUsername)
        {
            bool credentialsExist = _providerConfiguration.Providers.TryGetValue("ConmodoNO", out ProviderConfiguration conmodoConfiguration);

            if (!credentialsExist)
                throw new ApplicationException("Provider credentials was not found!");

            // TODO: Temp override for testing/debug purposes
            apiUsername = conmodoConfiguration!.ApiUsername;

            var provider = new Conmodo.ConmodoProviderServices(conmodoConfiguration.ApiBaseUrl, apiUsername, conmodoConfiguration.ApiPassword);
            return provider;
        }

    }
}
