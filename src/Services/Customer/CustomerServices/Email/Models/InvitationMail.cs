using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Email.Models
{
    public class InvitationMail
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// The link to guide the new user to Origo. 
        /// </summary>
        public string OrigoBaseUrl { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public List<string> Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Welcome to Origo!";
        public const string TemplateName = "OrigoInvitation";
    }
}
