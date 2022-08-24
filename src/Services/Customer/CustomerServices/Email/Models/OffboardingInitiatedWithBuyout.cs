using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Email.Models
{
    public class OffboardingInitiatedWithBuyout
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
        /// Last day of buy out
        /// </summary>
        public string LastBuyoutDay { get; set; }

        /// <summary>
        /// My page Link
        /// </summary>
        public string MyPageLink { get; set; }
        public string Subject { get; set; } = "Your Offboarding Tasks";
        public const string TemplateName = "OffboardingInitiatedWithBuyout";
    }
}
