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
        /// <param name="loanPhoneNumber">The phone-number in <c>E.164</c> format.</param>
        /// <param name="loanPhoneEmail"></param>
        /// <param name="callerId"></param>
        /// <returns></returns>
        Task<CustomerSettings> ConfigureServiceIdAsync(
            Guid customerId,
            List<int> assetCategoryIds,
            int providerId,
            string apiUsername,
            string loanPhoneNumber,
            string loanPhoneEmail,
            Guid callerId);

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
    }
}
