using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string Subject { get; set; } = "Asset unassigned";
        public const string TemplateName = "UnassignedFromUser";
    }
}
