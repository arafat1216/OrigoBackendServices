using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
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
        /// Subject of the email
        /// </summary>
        public const string Subject = "ManagerOnBehalfReturn_Subject";
        public const string TemplateName = "ManagerOnBehalfReturn";
    }
}
