using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetServices.Email.Model
{
    public class PendingReturnNotification
    {
        /// <summary>
        /// First name of the email recipient
        /// </summary>
        public string FirstName { get; set; }
        /// <summary>
        /// Origo Assets View Page Link
        /// </summary>
        public string AssetLink { get; set; }
        /// <summary>
        /// Email address of the recipient
        /// </summary>
        public IList<string> Recipients { get; set; }
        /// <summary>
        /// Customer id to be insurted in to the link for the asset lifecycle.
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// Asset Lifecycle id to be insurted in to the link for the asset lifecycle.
        /// </summary>
        public string AssetLifecycleId { get; set; }
        /// <summary>
        /// Subject of the email
        /// </summary>
        public const string Subject = "PendingReturn_Subject";
        public const string TemplateName = "PendingReturn";
        
    }
}
