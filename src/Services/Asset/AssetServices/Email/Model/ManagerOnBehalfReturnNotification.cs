using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
    /// <summary>
    /// Email Data to notify User when Manager perform 'Return' on behalf
    /// </summary>
    public class ManagerOnBehalfReturnNotification
    {
        /// <summary>
        /// Username of the asset owner
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Asset Id
        /// </summary>
        public string AssetId { get; set; }
        /// <summary>
        /// Asset Name
        /// </summary>
        public string AssetName { get; set; }
        /// <summary>
        /// Asset Return Date
        /// </summary>
        public string ReturnDate { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public string Recipient { get; set; }
        /// <summary>
        /// Name of the subject template in resource
        /// </summary>
        public const string SubjectTemplatename = "ManagerOnBehalfReturn_Subject";
        /// <summary>
        /// Name of the email body template in resource
        /// </summary>
        public const string TemplateName = "ManagerOnBehalfReturn";
    }
}
