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
        /// Link to the shipping label (or a page where it can be retrieved)
        /// </summary>
        public string ShippingLabelLink { get; set; }

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

        public const string TemplateKeyName = "AssetRepairEmail";

        public const string SubjectKeyName = "AssetRepairEmail_Subject";
    }
}
