namespace HardwareServiceOrderServices.Email.Models
{
    public class OrderConfirmationEmail
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Name of the asset
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// Unique identifier of the asset
        /// </summary>
        public string AssetId { get; set; }
        /// <summary>
        /// Date of the order
        /// </summary>
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Type of the repair
        /// </summary>
        public string RepairType { get; set; }
        /// <summary>
        /// Category of the fault
        /// </summary>
        public string FaultCategory { get; set; }
        /// <summary>
        /// Origo order link
        /// </summary>
        public string OrderLink { get; set; }
        /// <summary>
        /// Link of the package slip
        /// </summary>
        public string PackageSlipLink { get; set; }
        /// <summary>
        /// Loan device contact email
        /// </summary>
        public string LoanDeviceContact { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; }
        public const string TemplateName = "OrderConfirmationEmail";
        public const string SubjectName = "OrderConfirmationEmail_Subject";
    }
}
