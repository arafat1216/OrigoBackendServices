#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder
{

    /// <summary>
    /// Settings for managing hardware service order
    /// </summary>
    public class LegacyCustomerSettings
    {
        /// <summary>
        /// Customer identifier
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Identifier/Username of the service provider
        /// </summary>
        public string ServiceId { get; set; }
        /// <summary>
        /// Loan device configuration details
        /// </summary>
        public LoanDevice LoanDevice { get; set; }
    }
}
