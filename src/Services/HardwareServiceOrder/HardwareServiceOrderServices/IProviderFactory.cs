namespace HardwareServiceOrderServices
{
    public interface IProviderFactory
    {
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
        /// <exception cref="NotSupportedException"> Thrown if the <paramref name="providerId"/> don't offer repair services
        ///     or the provided value is otherwise invalid/unsupported. </exception>
        Task<IRepairProvider> GetRepairProviderAsync(int providerId, string? apiUsername = null, string? apiPassword = null);

        // This is not yet implemented (currently out of scope)
        //Task<IAftermarketProvider> GetAftermarketProviderAsync(int providerId, string? apiUsername = null, string? apiPassword = null);
    }
}
