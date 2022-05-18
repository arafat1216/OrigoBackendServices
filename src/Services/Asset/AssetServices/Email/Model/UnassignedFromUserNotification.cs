namespace AssetServices.Email.Model
{
    public class UnassignedFromUserNotification
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Unassigned Asset";
        public const string TemplateName = "UnassignedFromUser";
    }
}
