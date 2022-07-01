namespace HardwareServiceOrder.API.ViewModels
{
    /// <summary>
    /// Settings for managing hardware service order
    /// </summary>
    public class CustomerSettings
    {
        public CustomerSettings()
        {

        }

        /// <summary>
        /// Customer identifier
        /// </summary>
        public Guid? CustomerId { get; set; }

        /// <summary>
        /// Loan device configuration details
        /// </summary>
        public LoanDevice? LoanDevice { get; set; }
    }
}
