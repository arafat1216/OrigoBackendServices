namespace HardwareServiceOrderServices.Email.Models
{
    public class AssetRepairEmail
    {
        /// <summary>
        /// ID of the customer
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// Unique identifier of the order
        /// </summary>
        public Guid OrderId { get; set; }
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Origo order link
        /// </summary>
        public string OrderLink { get; set; }
        /// <summary>
        /// Link of the package slip
        /// </summary>
        public string PackageSlipLink { get; set; }
        /// <summary>
        /// Date of the order
        /// </summary>
        public DateTimeOffset OrderDate { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; }
        public const string TemplateName = "AssetRepairEmail";
        public const string SubjectName = "AssetRepairEmail_Subject";
    }
}
