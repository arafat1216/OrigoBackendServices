namespace AssetServices.Email.Model
{
    public class ReAssignedToUserNotification
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Origo Asset View Page Link
        /// </summary>
        public string AssetLink { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Customer id to be insurted in to the link for the asset lifecycle.
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// Asset Lifecycle id to be insurted in to the link for the asset lifecycle.
        /// </summary>
        public string AssetLifecycleId { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "New Asset";
        public const string TemplateName = "ReassignedToUser";
    }
}
