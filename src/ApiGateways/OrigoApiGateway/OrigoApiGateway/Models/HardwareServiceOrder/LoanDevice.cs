

#nullable enable

namespace OrigoApiGateway.Models.HardwareServiceOrder
{
    public class LoanDevice
    {
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        /// <summary>
        /// This property ensures whether a customer provides loan device
        /// </summary>
        public bool ProvidesLoanDevice { get; set; }
    }
}
