using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerServices.Email.Models
{
    public class OffboardingOverdueMail
    {
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public List<string> Recipient { get; set; }
        /// <summary>
        /// User name of the Overdued User
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Last working Day of the Overdued User
        /// </summary>
        public string LastWorkingDays { get; set; }
        /// <summary>
        /// Customer Id of the Overdued User
        /// </summary>
        public Guid CustomerId { get; set; }
        /// <summary>
        /// User Id of the Overdued User
        /// </summary>
        public Guid UserId { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Offboarding Overdue";
        public const string TemplateName = "OffboardingOverdued";
    }
}
