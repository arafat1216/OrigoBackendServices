#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    /// <summary>
    ///    Loan device configuration details 
    /// </summary>
    public class LoanDevice
    {
        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        /// <summary>
        /// This property ensures whether a customer provides loan device
        /// </summary>
        public bool ProvidesLoanDevice { get; set; }
    }
}
