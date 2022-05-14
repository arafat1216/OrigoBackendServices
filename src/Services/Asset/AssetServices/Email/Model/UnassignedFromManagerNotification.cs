using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
    public class UnassignedFromManagerNotification
    {
        /// <summary>
        /// Email address of the recipients
        /// </summary>
        public IList<string> Recipient { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public string Subject { get; set; } = "Reassigned Asset";
        public const string TemplateName = "UnassignedFromManager";

    }
}
