using System.Collections.Generic;

namespace CustomerServices.Email.Models
{
    public class OffboardingInitiatedWithoutBuyout
    {
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public List<string> Recipient { get; set; }

        /// <summary>
        /// First name of the User
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// My page Link
        /// </summary>
        public string MyPageLink { get; set; }
        public const string Subject = "OffboardingInitiatedWithoutBuyout_Subject";
        public const string TemplateName = "OffboardingInitiatedWithoutBuyout";
    }
}
