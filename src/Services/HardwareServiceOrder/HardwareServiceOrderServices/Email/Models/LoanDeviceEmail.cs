namespace HardwareServiceOrderServices.Email.Models
{
    public class LoanDeviceEmail
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        public string Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; }
        public const string TemplateName = "LoanDeviceEmail";
        public const string SubjectName = "LoanDeviceEmail_Subject";
    }
}
