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
        public async Task<IRepairProvider> GetRepairProviderAsync(int serviceProviderId, string? apiUsername = null, string? apiPassword = null)
        {
            ServiceProviderEnum serviceProvider = (ServiceProviderEnum)serviceProviderId;

            switch (serviceProvider)
            {
                case ServiceProviderEnum.ConmodoNo:
                    if (string.IsNullOrEmpty(apiUsername)) throw new ArgumentNullException(nameof(apiUsername));
                    else return GetConmodoNo(apiUsername);

                default:
                    throw new NotSupportedException("Repair-services is currently not supported for this service-provider.");
            }
        }


        /// <inheritdoc/>
        public async Task<IAftermarketProvider> GetAftermarketProviderAsync(int serviceProviderId, string? apiUsername = null, string? apiPassword = null)
        {
            ServiceProviderEnum serviceProvider = (ServiceProviderEnum)serviceProviderId;

            switch (serviceProvider)
            {
                case ServiceProviderEnum.ConmodoNo:
                    if (string.IsNullOrEmpty(apiUsername)) throw new ArgumentNullException(nameof(apiUsername));
                    else return GetConmodoNo(apiUsername);

                default:
                    throw new NotSupportedException("Aftermarket-services is currently not supported for this service-provider.");
            }
        }


        public async Task<IGenericProviderOfferings> GetGenericProviderAsync(int serviceProviderId, string? apiUsername = null, string? apiPassword = null)
        {
            ServiceProviderEnum serviceProvider = (ServiceProviderEnum)serviceProviderId;

            switch (serviceProvider)
            {
                case ServiceProviderEnum.ConmodoNo:
                    if (string.IsNullOrEmpty(apiUsername)) throw new ArgumentNullException(nameof(apiUsername));
                    else return GetConmodoNo(apiUsername);

                default:
                    throw new NotSupportedException("Aftermarket-services is currently not supported for this service-provider.");
            }
        }


        /// <summary>
        ///     Retrieve Conmodo's interface implementation(s).
        /// </summary>
        /// <param name="apiUsername"> The customer's API username. </param>
        /// <returns> The service-provider implementation that has implemented the provider-interfaces. </returns>
        /// <exception cref="ApplicationException"> Thrown when the configurations/secrets was not found. </exception>
        private Conmodo.ConmodoProviderServices GetConmodoNo(string apiUsername)
        {
            bool credentialsExist = _providerConfiguration.Providers.TryGetValue("ConmodoNO", out ProviderConfiguration conmodoConfiguration);

            if (!credentialsExist)
                throw new ApplicationException("Provider credentials was not found!");
            if (conmodoConfiguration is null) 
                throw new ApplicationException("The system's provider-configuration details has not been provided.");

            var provider = new Conmodo.ConmodoProviderServices(conmodoConfiguration.ApiBaseUrl, apiUsername, conmodoConfiguration.ApiPassword);
            return provider;
        }

    }
}
