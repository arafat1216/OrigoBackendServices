namespace HardwareServiceOrderServices.Models
{
    public interface IHardwareServiceOrderRepository
    {
        /// <summary>
        /// Configure customer settings
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="assetCategoryIds">List of asset categories supported by the service provider</param>
        /// <param name="providerId">Provider identifer</param>
        /// <param name="apiUsername">Username for calling provider's API</param>
        /// <param name="apiPassowrd">Password for calling provider's API</param>
        /// <param name="loanPhoneNumber">The phone-number in <c>E.164</c> format.</param>
        /// <param name="loanPhoneEmail"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<CustomerSettings> ConfigureServiceIdAsync(
            Guid customerId,
            List<int> assetCategoryIds,
            int providerId,
            string loanPhoneNumber,
            string loanPhoneEmail,
            Guid callerId,
            string? apiUsername = null,
            string? apiPassowrd = null);

        /// <summary>
        /// Configure Loan Phone
        /// </summary>
        /// <param name="customerId">Customer identifier</param>
        /// <param name="loanPhoneNumber">The phone-number in <c>E.164</c> format.</param>
        /// <param name="loanPhoneEmail"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<CustomerSettings> ConfigureLoanPhoneAsync(Guid customerId, string loanPhoneNumber, string loanPhoneEmail, Guid callerId);

        Task<CustomerSettings> GetSettingsAsync(Guid customerId);

        Task<string?> GetServiceIdAsync(Guid customerId);

        Task<IEnumerable<HardwareServiceOrder>> GetAllOrdersAsync(DateTime? olderThan = null, List<int>? statusIds = null);

        Task<HardwareServiceOrder> GetOrderByIdAsync(Guid orderId);

        Task<List<HardwareServiceOrder>> GetAllOrdersAsync(Guid customerId);
        /// <summary>
        /// Update the status of a service order
        /// </summary>
        /// <param name="orderId">Order Identifier</param>
        /// <param name="newStatus">New status <see cref="ServiceStatusEnum"/></param>
        /// <returns></returns>
        Task<HardwareServiceOrder> UpdateOrderStatusAsync(Guid orderId, ServiceStatusEnum newStatus);
        /// <summary>
        /// Get all customers' providers
        /// </summary>
        /// <returns></returns>
        Task<List<CustomerServiceProvider>> GetAllCustomerProvidersAsync();

        /// <summary>
        /// Get order by service provider's order ID
        /// </summary>
        /// <param name="serviceProviderOrderId">The identifier that was provided by the service-provider. <see cref="HardwareServiceOrder.ServiceProviderOrderId1"/></param>
        /// <returns></returns>
        Task<HardwareServiceOrder?> GetOrderByServiceProviderOrderIdAsync(string serviceProviderOrderId);

        /// <summary>
        /// Update Customer Provider's LastUpdateFetched
        /// </summary>
        /// <param name="customerServiceProvider">Existing Customer Service Provider <see cref="Models.CustomerServiceProvider"/></param>
        /// <param name="lastUpdateFetched">Last DateTime when the updates were fetched from the service provider</param>
        /// <returns></returns>
        Task UpdateCustomerProviderLastUpdateFetchedAsync(CustomerServiceProvider customerServiceProvider, DateTimeOffset lastUpdateFetched);

        /// <summary>
        /// Update service order's events
        /// </summary>
        /// <param name="order">Order to be updated</param>
        /// <param name="events">List of events</param>
        /// <returns></returns>
        Task UpdateServiceEventsAsync(HardwareServiceOrder order, IEnumerable<ServiceEvent> events);
    }
}
